using Godot;
using Godot.Collections;

public partial class PortView3D : Node3D
{
    private Array<Sprite3D> _art;
    private int _artIndex;
    private Camera _camera;

    private RandomNumberGenerator _rng;

    public override void _Ready()
    {
        _art = Tools.GetChildren<Sprite3D>(this);
        _camera = Tools.GetChild<Camera>(this);
        _rng = new RandomNumberGenerator();

        _artIndex = (int)(_rng.Randi() % _art.Count);
        _camera.Focus(_art[_artIndex]);
        _camera.SetCamera();
    }

    public override void _Process(double delta)
    {
        _camera.Focus(_art[_artIndex]);
        _camera._Process(delta);
    }

    public void ChangeArt(Texture2D art)
    {
        _art[_artIndex].Texture = art;
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