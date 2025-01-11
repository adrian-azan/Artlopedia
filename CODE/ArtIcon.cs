using Godot;
using Godot.Collections;
using System;

public partial class ArtIcon : Control
{
    private AspectRatioContainer _container;
    private TextureRect _background;
    private TextureRect _art;

    private Texture2D _highlighted;
    private Texture2D _normal;

    public string _id;
    public string _title;
    public float _rating;
    public string[] _tags;
    public string _locationPurchased;
    public float _width;
    public float _height;
    public float _orientation2D;
    public float _orientation3D;

    public override void _Ready()
    {
        _background = GetNode<TextureRect>("Background");
        _art = GetNode<TextureRect>("AspectRatioContainer/Art");
        _container = GetNode<AspectRatioContainer>("AspectRatioContainer");

        _highlighted = ResourceLoader.Load("res://ART/UI/ArtBackground Highlighted.png") as Texture2D;
        _normal = ResourceLoader.Load("res://ART/UI/ArtBackground.png") as Texture2D;
    }

    public void Init(string[] artDetails)
    {
        _art.Texture = ResourceLoader.Load(String.Format("res://ART/Your Art Here/{0}.JPG", artDetails[0])) as Texture2D;
        _title = artDetails[1];
        _id = artDetails[0];
        _rating = 1.0f;
    }

    public void ArtTexture(ArtIcon artIcon)
    {
        _art.Texture = artIcon._art.Texture;
        _container.Rotation = artIcon._container.Rotation;
    }

    public Texture2D ArtTexture()
    {
        return _art.Texture;
    }

    public void RotateClockwise()
    {
        _container.RotationDegrees += 90;
    }

    public void RotateCounterClockwise()
    {
        _container.RotationDegrees -= 90;
    }

    public void Highlight()
    {
        _background.Texture = _highlighted;
    }

    public void UnHighlight()
    {
        _background.Texture = _normal;
    }

    public void Deserialize(Dictionary artDetails)
    {
        _height = (float)artDetails["dimensions"].AsGodotDictionary()["height"];
        _width = (float)artDetails["dimensions"].AsGodotDictionary()["width"];

        _orientation2D = (float)artDetails["orientation"].AsGodotDictionary()["2D"];
        _orientation3D = (float)artDetails["orientation"].AsGodotDictionary()["3D"];

        _container.RotationDegrees = _orientation2D;

        _id = artDetails["id"].AsString();
        _locationPurchased = artDetails["locationPurchased"].AsString();
        _rating = (float)artDetails["rating"];
        _tags = artDetails["tags"].AsStringArray();
        _title = artDetails["title"].AsString();
    }

    public Dictionary Serialize()
    {
        Dictionary output = new Dictionary();
        Dictionary dimensions = new Dictionary();
        dimensions.Add("width", _width);
        dimensions.Add("height", _height);
        Dictionary orientation = new Dictionary();
        orientation.Add("2D", _container.RotationDegrees);
        orientation.Add("3D", _orientation3D);

        output.Add("title", _title);
        output.Add("id", _id);
        output.Add("rating", _rating);
        output.Add("tags", _tags);
        output.Add("locationPurchased", _locationPurchased);
        output.Add("dimensions", dimensions);
        output.Add("orientation", orientation);

        return output;
    }
}