using Godot;
using System;

public partial class RootWindow : Node2D
{
    private IconCollection _iconCollection;
    private PortView3D _portView3D;

    private RichTextLabel _artTitle;
    private RichTextLabel _artId;

    public override void _Ready()
    {
        _iconCollection = GetNode<IconCollection>("IconCollection");
        _portView3D = GetNode<PortView3D>("SubViewportContainer/SubViewport/3dView");
        _artTitle = GetNode<RichTextLabel>("ArtTitle/RichTextLabel");
        _artId = GetNode<RichTextLabel>("ArtId/Control/ID Number");
    }

    public override void _Process(double delta)
    {
        ArtIcon currentFocus = _iconCollection.FocusedArtIcon();

        _portView3D.ChangeArt(currentFocus.ArtTexture());
        _artTitle.Text = currentFocus._title;
        _artId.Text = currentFocus._id;

        if (Input.IsActionJustPressed("RotateClockwise3D"))
            _portView3D.RotateClockwise();

        if (Input.IsActionJustPressed("RotateCounterClockwise3D"))
            _portView3D.RotateCounterClockwise();
    }
}