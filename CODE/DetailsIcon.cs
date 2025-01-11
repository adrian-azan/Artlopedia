using Godot;
using System;

public partial class DetailsIcon : Control
{
    private Texture2D _highlighted;
    private Texture2D _normal;

    public override void _Ready()
    {
        _normal = ResourceLoader.Load<Texture2D>("res://ART/UI/TitleBackground.png");
        _highlighted = ResourceLoader.Load<Texture2D>("res://ART/UI/TitleBackground Highlighted.png");
    }

    public void Highlight()
    {
        if (_highlighted == null) return;
        GetNode<Sprite2D>("Sprite2D").Texture = _highlighted;
    }

    public void UnHighlight()
    {
        if (_normal == null) return;
        GetNode<Sprite2D>("Sprite2D").Texture = _normal;
    }
}