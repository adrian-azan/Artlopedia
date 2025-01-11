using Godot;
using Godot.Collections;

public partial class RightPanel : Node2D
{
    private Array<PortView3D> _portView3D;
    private int _currentPortView;

    private RichTextLabel _artTitle;
    private RichTextLabel _artId;
    private StarRating _starRating;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _portView3D = Tools.GetChildren<PortView3D>(this);
        _currentPortView = 0;
        _portView3D[_currentPortView].Show();
        _artTitle = GetNode<RichTextLabel>("ArtTitle/RichTextLabel");
        _artId = GetNode<RichTextLabel>("ArtId/Control/ID Number");
        _starRating = GetNode<StarRating>("StarRating");

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

        var currentFocus = GetViewport().GuiGetFocusOwner();
        foreach (DetailsIcon details in Tools.GetChildren<DetailsIcon>(this))
        {
            if (details == currentFocus)
                details.Highlight();
            else
                details.UnHighlight();
        }
    }

    public void ProcessInput(ArtIcon _currentFocus)
    {
        if (Input.IsActionJustPressed("South RightThumb") && GetNode("StarRating") == GetViewport().GuiGetFocusOwner())
        {
            GetNode<StarRating>("StarRating").Increase();
            _currentFocus._rating = _starRating._rating;
        }

        if (Input.IsActionJustPressed("East RightThumb") && GetNode("StarRating") == GetViewport().GuiGetFocusOwner())
        {
            GetNode<StarRating>("StarRating").Decrease();
            _currentFocus._rating = _starRating._rating;
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
        _starRating.SetRating(currentFocus._rating);
    }
}