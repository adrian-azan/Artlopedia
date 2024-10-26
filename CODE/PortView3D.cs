using Godot;
using System;

public partial class PortView3D : Node3D
{
    private Sprite3D _art;

    private Camera3D _camera;
    private MeshInstance3D _cameraCover;
    private Timer _fadeTimer;
    private float _cameraSpeedX;
    private float _cameraSpeedY;
    private float _panDuration;
    private float _zRadius;
    private Vector3 _cameraStartPosition;
    private Vector3 _cameraEndPosition;

    private RandomNumberGenerator _rng;

    public override void _Ready()
    {
        _art = GetNode<Sprite3D>("Sprite3D");
        _camera = GetNode<Camera3D>("Camera3D");
        _cameraCover = GetNode<MeshInstance3D>("Camera3D/MeshInstance3D");
        _rng = new RandomNumberGenerator();

        _fadeTimer = GetNode<Timer>("Camera3D/Timer");

        SetCamera();
    }

    public override void _Process(double delta)
    {
        Vector3 currentPosition = _camera.Position;
        currentPosition.X -= (float)(_cameraSpeedX * delta);
        currentPosition.Y -= (float)(_cameraSpeedY * delta);
        currentPosition.Z = Mathf.Sin((Mathf.Pi) * Mathf.Abs((currentPosition.X - _cameraStartPosition.X) / (_cameraEndPosition.X * 2))) * _zRadius;

        _camera.Position = currentPosition;
        _camera.LookAt(_art.Position);

        if (Mathf.Abs(_camera.Position.X - _cameraEndPosition.X) <= .65f && _fadeTimer.IsStopped())
        {
            (GetNode("Camera3D/MeshInstance3D/AnimationPlayer") as AnimationPlayer).Play("FadeOut");
            _fadeTimer.Start();
        }
    }

    public void SetCamera()
    {
        _fadeTimer.Stop();
        int side = 1;
        if (_rng.RandiRange(0, 1) == 0)
            side = -1;

        _camera.Position = new Vector3(_rng.RandiRange(side * 4, side * 8), _rng.RandiRange(8, 14), 0);
        _cameraEndPosition = new Vector3(-_camera.Position.X, _rng.RandiRange(8, 14), 0);
        _cameraStartPosition = _camera.Position;

        _panDuration = _rng.RandiRange(7, 20);
        _zRadius = _rng.RandiRange(1, 8);
        _cameraSpeedX = (_cameraStartPosition.X - _cameraEndPosition.X) / _panDuration;
        _cameraSpeedY = Mathf.Abs(_cameraStartPosition.Y - _cameraEndPosition.Y) / _panDuration;

        (GetNode("Camera3D/MeshInstance3D/AnimationPlayer") as AnimationPlayer).Play("FadeIn");
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