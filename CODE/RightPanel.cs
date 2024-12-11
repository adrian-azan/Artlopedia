using Godot;
using Godot.Collections;

public partial class RightPanel : Node2D
{
    private Array<PortView3D> _portView3D;
    private int _currentPortView;

    private RichTextLabel _artTitle;
    private RichTextLabel _artId;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _portView3D = Tools.GetChildren<PortView3D>(this);
        _currentPortView = 0;
        _portView3D[_currentPortView].Show();
        _artTitle = GetNode<RichTextLabel>("ArtTitle/RichTextLabel");
        _artId = GetNode<RichTextLabel>("ArtId/Control/ID Number");

        _animationPlayer = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _animationPlayer.Stop();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ChangeRoom"))
        {
            _portView3D[_currentPortView].Hide();
            _currentPortView++;
            _currentPortView %= _portView3D.Count;
            _portView3D[_currentPortView].Show();
            _portView3D[_currentPortView].MakeCurrent();
        }
    }

    public void Focus3DView()
    {
        _animationPlayer.Play("Grow");
    }

    public void UnFocus3DView()
    {
        _animationPlayer.Play("Shrink");
    }

    public bool Busy()
    {
        return _animationPlayer.IsPlaying();
    }

    public void SetFocusedArt(ArtIcon currentFocus)
    {
        _portView3D[_currentPortView].ChangeArt(currentFocus.ArtTexture());
        _artTitle.Text = currentFocus._title;
        _artId.Text = currentFocus._id;
    }
}