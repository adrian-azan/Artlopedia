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

		CustomSignals._Instance.Rotate3DArt += SetRotation;
	}

	public override void _Process(double delta)
	{
		_camera.Focus(_art[_artIndex]);
		_camera._Process(delta);
	}

	public void MakeCurrent()
	{
		_camera.MakeCurrent();
	}

	public void ChangeArt(Texture2D art)
	{
		CreateTween().TweenProperty(_art[_artIndex], "transparency", 1, .1f);
		CreateTween().TweenProperty(_art[_artIndex], "transparency", 0, .2f);
		
		_art[_artIndex].Texture = art;
	}

	public void SetRotation(float rotationZ)
	{
		_art[_artIndex].RotationDegrees = new Vector3(-20, 0, rotationZ);
	}
}
