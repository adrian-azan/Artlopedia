using Godot;
using System;

public partial class Camera_Panning : Camera
{
    private float _cameraSpeedX;
    private float _cameraSpeedY;
    private Vector3 _cameraStartPosition;
    private Vector3 _cameraEndPosition;
    private float _zRadius;

    private RandomNumberGenerator _rng;

    public override void _Ready()
    {
        base._Ready();
        _animationPlayer.Play("FadeOut");

        _rng = new RandomNumberGenerator();
    }

    public override void _Process(double delta)
    {
        Vector3 currentPosition = _camera.Position;
        currentPosition.X -= (float)(_cameraSpeedX * delta);
        currentPosition.Y -= (float)(_cameraSpeedY * delta);
        currentPosition.Z = Mathf.Sin((Mathf.Pi) * Mathf.Abs((currentPosition.X - _cameraStartPosition.X) / (_cameraEndPosition.X * 2))) * _zRadius;

        _camera.Position = currentPosition;
        _camera.LookAt(_subject.GlobalPosition);

        if (Mathf.Abs(_camera.Position.X - _cameraEndPosition.X) <= .65f && _fadeTimer.IsStopped())
        {
            _animationPlayer.Play("FadeOut");
            _fadeTimer.Start();
        }
    }

    public override void Focus(Sprite3D subject)
    {
        _subject = subject;
    }

    public override void SetCamera()
    {
        float _panDuration = _rng.RandiRange(7, 20);
        float subjectX = _subject.GlobalPosition.X;
        float subjectY = _subject.GlobalPosition.Y;

        _fadeTimer.Stop();
        int side = 1;
        if (_rng.RandiRange(0, 1) == 0)
            side = -1;

        _camera.Position = new Vector3(subjectX + _rng.RandiRange(side * 4, side * 8), subjectY + _rng.RandiRange(-9, 0), 1);
        _cameraEndPosition = new Vector3(-_camera.Position.X, subjectY + _rng.RandiRange(-9, 0), 1);
        _cameraStartPosition = _camera.Position;

        _zRadius = _rng.RandiRange(2, 6);
        _cameraSpeedX = (_cameraStartPosition.X - _cameraEndPosition.X) / _panDuration;
        _cameraSpeedY = Mathf.Abs(_cameraStartPosition.Y - _cameraEndPosition.Y) / _panDuration;

        _animationPlayer.Play("FadeIn");
    }
}