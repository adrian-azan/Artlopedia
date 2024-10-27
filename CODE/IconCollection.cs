using Godot;
using Godot.Collections;

public partial class IconCollection : Node
{
    private int row;
    private int col;

    private int _lastFilledCol;
    private int _lastFilledRow;
    private Array<VBoxContainer> _allIcons;
    private PortView3D _portView3D;

    public override void _Ready()
    {
        row = 0;
        col = 0;
        _portView3D = GetNode<PortView3D>("../RightPanel/SubViewportContainer/SubViewport/3dView");
        _allIcons = Variant.From(GetNode("HBoxContainer").GetChildren()).AsGodotArray<VBoxContainer>();

        var artDetailsMasterList = FileAccess.Open("res://ART/Your Art Here/Master List.csv", FileAccess.ModeFlags.Read);

        string[] artDetails = artDetailsMasterList.GetCsvLine(); //Ignore Header
        artDetails = artDetailsMasterList.GetCsvLine();

        int columnToAddTo = 0;

        do
        {
            ArtIcon artIcon = ResourceLoader.Load<PackedScene>("res://SCENES/UI/ArtIcon.tscn").Instantiate() as ArtIcon;
            _allIcons[columnToAddTo++].AddChild(artIcon);
            artIcon.Init(artDetails);

            columnToAddTo %= _allIcons.Count;
            artDetails = artDetailsMasterList.GetCsvLine();
            break;
        } while (artDetailsMasterList.EofReached() == false);

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

    public void Down()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        row += 1;
        if (row >= _allIcons[col].GetChildren().Count)
            row = 0;

        (_allIcons[col].GetChildren()[row] as ArtIcon).Highlight();
    }

    public void Up()
    {
        (_allIcons[col].GetChildren()[row] as ArtIcon).UnHighlight();

        row -= 1;
        if (row < 0)
            row = _allIcons[col].GetChildren().Count - 1;

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