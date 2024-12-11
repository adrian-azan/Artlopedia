using Godot;
using System;

public partial class Camera : Node3D
{
    protected Camera3D _camera;
    protected Sprite3D _subject;

    protected AnimationPlayer _animationPlayer;
    protected MeshInstance3D _lensCover;
    protected Timer _fadeTimer;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        _lensCover = _camera.GetNode<MeshInstance3D>("MeshInstance3D");
        _fadeTimer = _camera.GetNode<Timer>("Timer");

        _animationPlayer = _camera.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public virtual void Focus(Sprite3D subject)
    { }

    public virtual void SetCamera()
    { }
}