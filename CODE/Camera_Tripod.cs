using Godot;
using System;

public partial class Camera_Tripod : Camera
{
    public override void _Process(double delta)
    {
        if (_animationPlayer.IsPlaying() == false)
            _animationPlayer.Play("Rotate");
    }

    public override void SetCamera()
    {
        GD.Print("Setting");
        _animationPlayer.Play("FadeIn");
        _animationPlayer.Play("Rotate");
    }

    public override void Focus(Sprite3D subject)
    {
    }
}