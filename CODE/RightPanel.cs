using Godot;

public partial class RightPanel : Node2D
{
    private PortView3D _portView3D;

    private RichTextLabel _artTitle;
    private RichTextLabel _artId;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _portView3D = GetNode<PortView3D>("SubViewportContainer/SubViewport/3dView");
        _artTitle = GetNode<RichTextLabel>("ArtTitle/RichTextLabel");
        _artId = GetNode<RichTextLabel>("ArtId/Control/ID Number");

        _animationPlayer = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _animationPlayer.Stop();
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
        _portView3D.ChangeArt(currentFocus.ArtTexture());
        _artTitle.Text = currentFocus._title;
        _artId.Text = currentFocus._id;
    }
}