using Godot;
using System;

public partial class PortView3D : Node3D
{
    private Sprite3D _art;

    public override void _Ready()
    {
        _art = GetNode<Sprite3D>("Sprite3D");
    }

    public void ChangeArt(Texture2D art)
    {
        _art.Texture = art;
    }

    public void RotateClockwise()
    {
        Sprite3D art = GetNode("Sprite3D") as Sprite3D;

        art.RotationDegrees = new Vector3(-20, 0, art.RotationDegrees.Z - 90);
    }

    public void RotateCounterClockwise()
    {
        Sprite3D art = GetNode("Sprite3D") as Sprite3D;

        art.RotationDegrees = new Vector3(-20, 0, art.RotationDegrees.Z + 90);
    }
}