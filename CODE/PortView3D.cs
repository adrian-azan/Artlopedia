using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PortView3D : Node3D
{
    private Sprite3D _art;
    private Camera _camera;

    public override void _Ready()
    {
        _art = GetNode<Sprite3D>("SpotLightRoom/Sprite3D");
        _camera = GetNode<Camera>("Camera");
        _camera.Focus(_art);
        _camera.SetCamera();
    }

    public override void _Process(double delta)
    {
        _camera._Process(delta, _art);
    }

    public void ChangeArt(Texture2D art)
    {
        _art.Texture = art;
    }

    public void RotateClockwise()
    {
        Sprite3D art = GetNode("SpotLightRoom/Sprite3D") as Sprite3D;

        art.RotationDegrees = new Vector3(-20, 0, art.RotationDegrees.Z - 90);
    }

    public void RotateCounterClockwise()
    {
        Sprite3D art = GetNode("SpotLightRoom/Sprite3D") as Sprite3D;

        art.RotationDegrees = new Vector3(-20, 0, art.RotationDegrees.Z + 90);
    }
}