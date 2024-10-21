using Godot;
using System;

public partial class RootWindow : Node2D
{
    private IconCollection _iconCollection;
    private PortView3D _portView3D;

    public override void _Ready()
    {
        _iconCollection = GetNode<IconCollection>("IconCollection");
        _portView3D = GetNode<PortView3D>("SubViewportContainer/SubViewport/3dView");
    }

    public override void _Process(double delta)
    {
        _portView3D.ChangeArt(_iconCollection.FocusedArtIcon().ArtTexture());

        if (Input.IsActionJustPressed("RotateClockwise3D"))
            _portView3D.RotateClockwise();

        if (Input.IsActionJustPressed("RotateCounterClockwise3D"))
            _portView3D.RotateCounterClockwise();
    }
}