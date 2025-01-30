using Godot;
using Godot.Collections;
using System;
using System.Linq;

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
    private PortView3D _portView3D;

    public override void _Ready()
    {
        row = 0;
        col = 0;
        _portView3D = Tools.GetChild<PortView3D>(GetNode(".."));
        _allIcons = Variant.From(GetNode("HBoxContainer").GetChildren()).AsGodotArray<VBoxContainer>();
        _originalPos = Position;

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        InitIcons();
        totalLength = -75 * (_allIcons[col].GetChildren().Count - 4);
    }

    private void InitIcons()
    {
        string[] allImages = DirAccess.GetFilesAt("res://ART/Your Art Here").Where(fileName => fileName.Contains(".import") == false).ToArray();

        int columnToAddTo = 0;
        foreach (var artId in allImages)
        {
            Dictionary artDetails = null;

            //Check if a details file exists for current art piece
            FileAccess fa = FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", artId.Split(".")[0]), FileAccess.ModeFlags.Read);
            if (fa != null)
                artDetails = Json.ParseString(fa.GetAsText()).AsGodotDictionary();
            //If there is no details file, create file with default art details
            else
            {
                artDetails = new Dictionary();
                Dictionary dimensionsDefault = new Dictionary();
                dimensionsDefault.Add("width", 0);
                dimensionsDefault.Add("height", 0);
                Dictionary orientationDefault = new Dictionary();
                orientationDefault.Add("2D", 0);
                orientationDefault.Add("3D", 0);

                artDetails.Add("title", "");
                artDetails.Add("id", "00");
                artDetails.Add("rating", 2.5);
                artDetails.Add("tags", new string[0]);
                artDetails.Add("locationPurchased", "");
                artDetails.Add("dimensions", dimensionsDefault);
                artDetails.Add("orientation", orientationDefault);

                //Check if artpiece has a valid hexidecimal name
                string id = artId.Split(".")[0];
                if (id.IsValidHexNumber() == false)
                {
                    //Find a valid hex name that does not exist and rename art
                    var rng = new RandomNumberGenerator();
                    do
                    {
                        id = rng.RandiRange(0, 4095).ToString("X");
                    } while (FileAccess.FileExists(String.Format("res://ART/Your Art Here/Details/{0}.txt", id)));
                    DirAccess.RenameAbsolute(String.Format("res://ART/Your Art Here/{0}", artId), String.Format("res://ART/Your Art Here/{0}.JPG", id));
                }

                //Create details file with now valid id
                artDetails["id"] = id;
                DirAccess.RenameAbsolute(String.Format("res://ART/Your Art Here/{0}.import", artId), String.Format("res://ART/Your Art Here/{0}.JPG.import", id));
                var newDetails = FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", id), FileAccess.ModeFlags.Write);
                newDetails.Close();
            }

            ArtIcon artIcon = ResourceLoader.Load<PackedScene>("res://SCENES/UI/ArtIcon.tscn").Instantiate() as ArtIcon;
            _allIcons[columnToAddTo++].AddChild(artIcon);

            artIcon.Deserialize(artDetails);
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
            Up();

        if (Input.IsActionJustPressed("Down"))
            Down();

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

    public ArtIcon FocusedArtIcon()
    {
        return _allIcons[col].GetChildren()[row] as ArtIcon;
    }

    public void SAVE(Array<Dictionary> artIcons)
    {
        foreach (var art in artIcons)
        {
            using var fout = FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", art["id"]), FileAccess.ModeFlags.Write);
            fout.StoreString(Json.Stringify(art, "\t"));
            fout.Close();
        }
    }

    public void LOAD()
    {
        foreach (var column in _allIcons)
        {
            var columnArt = column.GetChildren();

            foreach (var art in columnArt)
            {
                if (art is ArtIcon)
                {
                    ArtIcon artIcon = (art as ArtIcon);
                    FileAccess fa = FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", artIcon._id), FileAccess.ModeFlags.Read);
                    if (fa != null)
                    {
                        Dictionary artDetails = Json.ParseString(fa.GetAsText(true)).AsGodotDictionary();
                        artIcon.Deserialize(artDetails);
                    }
                }
            }
        }
    }

    public void AllArt(Dictionary<string, Dictionary> allArtDetails)
    {
        foreach (var column in _allIcons)
        {
            var columnArt = column.GetChildren();

            foreach (var art in columnArt)
            {
                if (art is ArtIcon)
                {
                    (art as ArtIcon).Deserialize(allArtDetails[(art as ArtIcon)._id]);
                }
            }
        }
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