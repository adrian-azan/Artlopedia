using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class IconCollection : Node
{
    private int row;
    private int col;

    private int _lastFilledCol;
    private int _lastFilledRow;
    private Array<VBoxContainer> _allIcons;
    private PortView3D _portView3D;
    private static Dictionary DEFAULT_DETAILS;

    public override void _Ready()
    {
        row = 0;
        col = 0;
        _portView3D = Tools.GetChild<PortView3D>(GetNode(".."));
        _allIcons = Variant.From(GetNode("HBoxContainer").GetChildren()).AsGodotArray<VBoxContainer>();

        DEFAULT_DETAILS = new Dictionary();
        Dictionary output = new Dictionary();
        Dictionary dimensions = new Dictionary();
        dimensions.Add("width", 0);
        dimensions.Add("height", 0);
        Dictionary orientation = new Dictionary();
        orientation.Add("2D", 0);
        orientation.Add("3D", 0);

        output.Add("title", "");
        output.Add("id", "00");
        output.Add("rating", 2.5);
        output.Add("tags", new string[0]);
        output.Add("locationPurchased", "");
        output.Add("dimensions", dimensions);
        output.Add("orientation", orientation);

        InitIcons();
    }

    private void InitIcons()
    {
        string[] allImages = DirAccess.GetFilesAt("res://ART/Your Art Here").Where(fileName => fileName.Contains(".import") == false).ToArray();

        int columnToAddTo = 0;
        foreach (var artId in allImages)
        {
            Dictionary artDetails = null;

            FileAccess fa = FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", artId.Split(".")[0]), FileAccess.ModeFlags.Read);
            if (fa != null)
                artDetails = Json.ParseString(fa.GetAsText(true)).AsGodotDictionary();
            else
            {
                artDetails = DEFAULT_DETAILS;
                var rng = new RandomNumberGenerator();
                string id;
                do
                {
                    id = rng.RandiRange(0, 4095).ToString();
                } while (FileAccess.Open(String.Format("res://ART/Your Art Here/Details/{0}.txt", id), FileAccess.ModeFlags.Read) != null);

                artDetails["id"] = id;
            }

            ArtIcon artIcon = ResourceLoader.Load<PackedScene>("res://SCENES/UI/ArtIcon.tscn").Instantiate() as ArtIcon;
            _allIcons[columnToAddTo++].AddChild(artIcon);

            Dictionary output = new Dictionary();
            Dictionary dimensions = new Dictionary();
            dimensions.Add("width", artDetails["dimensions"].AsGodotDictionary()["width"]);
            dimensions.Add("height", artDetails["dimensions"].AsGodotDictionary()["height"]);
            Dictionary orientation = new Dictionary();
            orientation.Add("2D", artDetails["orientation"].AsGodotDictionary()["2D"]);
            orientation.Add("3D", artDetails["orientation"].AsGodotDictionary()["3D"]);

            output.Add("title", artDetails["title"]);
            output.Add("id", artId.Split(".")[0]);
            output.Add("rating", artDetails["rating"]);
            output.Add("tags", artDetails["tags"]);
            output.Add("locationPurchased", artDetails["locationPurchased"]);
            output.Add("dimensions", dimensions);
            output.Add("orientation", orientation);

            artIcon.Deserialize(output);
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