using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class IconCollection : Node2D
{
    private int row;
    private int col;
    private float totalLength;

    private Vector2 _originalPos;
    private AnimationPlayer _animationPlayer;

    private int _lastFilledCol;
    private int _lastFilledRow;
    private Array<VBoxContainer> _allIcons;
    private Dictionary<string, ArtIcon> _allDetails;
    private PortView3D _portView3D;

    [Export(PropertyHint.Range, "0,1,0.2")]
    public float preloadBufferPercentage;

    public override void _Ready()
    {
        row = 4;
        col = 0;
        _portView3D = Tools.GetChild<PortView3D>(GetNode(".."));
        _allIcons = Variant.From(GetNode("HBoxContainer").GetChildren()).AsGodotArray<VBoxContainer>();
        _allDetails = new Dictionary<string, ArtIcon>();
        _originalPos = Position;

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        totalLength = -75 * (_allIcons[col].GetChildren().Count - 4);
    }

    public void InitIcons()
    {
        string[] allImages = DirAccess.GetFilesAt(FileManager.ArtDirectory());

        int columnToAddTo = 0;
        foreach (var artFileName in allImages)
        {
            ArtIcon artIcon = ResourceLoader.Load<PackedScene>("res://SCENES/UI/ArtIcon.tscn").Instantiate() as ArtIcon;
            _allIcons[columnToAddTo++].AddChild(artIcon);
            _allDetails.Add(artFileName.Split(".")[0], artIcon);

            columnToAddTo %= _allIcons.Count;
        }

        if (columnToAddTo == 0)
            _lastFilledCol = _allIcons.Count - 1;
        else
            _lastFilledCol = columnToAddTo - 1;

        _lastFilledRow = _allIcons[_lastFilledCol].GetChildren().Count - 1;

        col = 0;
        (_allIcons[0].GetChildren()[0] as ArtIcon).Highlight();
    }

    public void _Control(double delta)
    {
        if (Input.IsActionJustPressed("Up"))
        {
            Up();
            for (int i = 0; i < 3; i++)
            {
                if (row - 10 >= 0)
                    (_allIcons[i].GetChildren()[row - 10] as ArtIcon).LoadLowResolution();
                if (row + 10 < _allIcons[i].GetChildren().Count)
                    (_allIcons[i].GetChildren()[row + 10] as ArtIcon).Clear();
            }
            GetNode<Timer>("Refresh").Start();
        }

        if (Input.IsActionJustPressed("Down"))
        {
            Down();
            for (int i = 0; i < 3; i++)
            {
                if (row + 10 < _allIcons[i].GetChildren().Count)
                    (_allIcons[i].GetChildren()[row + 10] as ArtIcon).LoadLowResolution();
                if (row - 10 >= 0)
                    (_allIcons[i].GetChildren()[row - 10] as ArtIcon).Clear();
            }
            GetNode<Timer>("Refresh").Start();
        }

        if (Input.IsActionJustPressed("Right"))
            Right();

        if (Input.IsActionJustPressed("Left"))
            Left();

        if (Input.IsActionJustPressed("RotateClockwise"))
            (_allIcons[col].GetChildren()[row] as ArtIcon).RotateClockwise();

        if (Input.IsActionJustPressed("RotateCounterClockwise"))
            (_allIcons[col].GetChildren()[row] as ArtIcon).RotateCounterClockwise();

        if (Input.IsActionJustPressed("RotateClockwise3D"))
            _portView3D.RotateClockwise();

        if (Input.IsActionJustPressed("RotateCounterClockwise3D"))
            _portView3D.RotateCounterClockwise();
    }

    public async void COLLECT()
    {
        await ToSignal((GetTree().CreateTimer(1)), SceneTreeTimer.SignalName.Timeout);
        GC.Collect();
    }

    public ArtIcon FocusedArtIcon()
    {
        return null;//_allIcons[col].GetChildren()[row] as ArtIcon;
    }

    public void SetDetails(Dictionary detail)
    {
        _allDetails[detail["id"].ToString()].Deserialize(detail);
    }

    public Array<ArtIcon> AllArt()
    {
        Array<ArtIcon> allArt = new Array<ArtIcon>();
        foreach (var column in _allIcons)
        {
            var columnArt = column.GetChildren();

            foreach (var art in columnArt)
            {
                if (art is ArtIcon)
                    allArt.Add(art as ArtIcon);
            }
        }

        return allArt;
    }

    public void PreLoadIcons()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 10 && j < _allIcons[i].GetChildCount(); j++)
            {
                (_allIcons[i].GetChildren()[j] as ArtIcon).LoadLowResolution();
            }
        }
        /*   var grid = new Array<Array<ArtIcon>>();

           for (int i = 0; i < _allIcons.Count; i++)
           {
               var iconsInAColumn = Tools.GetChildren<ArtIcon>(_allIcons[i]);
               grid.Add(new Array<ArtIcon>());
               for (int j = 0; j < iconsInAColumn.Count; j++)
               {
                   grid[i].Add(iconsInAColumn[j]);
               }
           }

           int rowsToKeep = 20;//(int)(_lastFilledRow * preloadBufferPercentage);
           rowsToKeep = rowsToKeep < 5 ? 20 : rowsToKeep;

           int startOverflow = -1;

           int start = (int)(row - rowsToKeep / 2);
           if (start < 0)
           {
               startOverflow = Math.Abs(start);
               start = 0;
           }

           int endOverflow = 0;
           int end = (int)(row + rowsToKeep / 2);

           GetNode<Label>("../end").Text = end.ToString();
           GetNode<Label>("../start").Text = start.ToString();

           for (int i = 0; i < grid.Count; i++)
           {
               for (int j = 0; j < grid[i].Count; j++)
               {
                   if ((j >= start && j <= end))
                       grid[i][j].LoadLowResolution();
                   else
                   {
                       grid[i][j].Clear();
                   }
               }
           }
           COLLECT();
        */
    }

    public void Down()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        row += 1;
        if (row >= _allIcons[col].GetChildren().Count)
        {
            row = 0;
            Position = _originalPos;
        }

        if ((_allIcons[col].GetChildren()[row] as ArtIcon).GlobalPosition.Y >= 300)
            CreateTween().TweenProperty(this, "position", new Vector2(Position.X, Position.Y - 100), .2);

        (_allIcons[col].GetChildren()[row] as ArtIcon).Highlight();
    }

    public void Up()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        row -= 1;
        if (row < 0)
        {
            row = _allIcons[col].GetChildren().Count - 1;
            Position = _originalPos + new Vector2(0, -75 * (_allIcons[col].GetChildren().Count - 4));
        }

        if ((_allIcons[col].GetChildren()[row] as ArtIcon).GlobalPosition.Y <= -400)
            CreateTween().TweenProperty(this, "position", new Vector2(Position.X, Position.Y + 100), .2);

        (_allIcons[col].GetChildren()[row] as ArtIcon).Highlight();
    }

    public void Left()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        col -= 1;
        if (col < 0)
        {
            if (row == _lastFilledRow)
                col = _lastFilledCol;
            else
                col = _allIcons.Count - 1;
        }

        (_allIcons[col].GetChildren()[row] as ArtIcon).Highlight();
    }

    public void Right()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        if (col >= _allIcons.Count - 1 || (row == _lastFilledRow && col == _lastFilledCol))
            col = 0;
        else
            col = (col + 1) % _allIcons.Count;

        (_allIcons[col].GetChildren()[row] as ArtIcon).Highlight();
    }
}