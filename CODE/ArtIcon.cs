using Godot;
using System;

public partial class ArtIcon : Control
{
    private AspectRatioContainer _container;
    private TextureRect _background;
    private TextureRect _art;

    private Texture2D _highlighted;
    private Texture2D _normal;

    public override void _Ready()
    {
        _background = GetNode<TextureRect>("Background");
        _art = GetNode<TextureRect>("AspectRatioContainer/Art");
        _container = GetNode<AspectRatioContainer>("AspectRatioContainer");

        _highlighted = ResourceLoader.Load("res://ART/UI/ArtBackground Highlighted.png") as Texture2D;
        _normal = ResourceLoader.Load("res://ART/UI/ArtBackground.png") as Texture2D;
    }

    public void LoadArt(string fileTitle)
    {
        _art.Texture = ResourceLoader.Load(String.Format("res://ART/PRINTS DONT COMMIT/{0}.JPG", fileTitle)) as Texture2D;
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
        (GetNode("AspectRatioContainer") as AspectRatioContainer).RotationDegrees += 90;
    }

    public void RotateCounterClockwise()
    {
        (GetNode("AspectRatioContainer") as AspectRatioContainer).RotationDegrees -= 90;
    }

    public void Highlight()
    {
        _background.Texture = _highlighted;
    }

    public void UnHighlight()
    {
        _background.Texture = _normal;
    }
}