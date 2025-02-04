using Godot;
using Godot.Collections;
using System;

public partial class RightPanel : Node2D
{
    private Array<PortView3D> _portView3D;
    private int _currentPortView;

    private RichTextLabel _artTitle;
    private RichTextLabel _artId;
    private RichTextLabel _artHeight;
    private RichTextLabel _artWidth;
    private StarRating _starRating;

    private Control _keyboardInput;
    private Control _keyboardInputSize;

    private AnimationPlayer _animationPlayer;
    private bool _typing;
    private Node _selectedDetail;

    public override void _Ready()
    {
        _portView3D = Tools.GetChildren<PortView3D>(this);
        _currentPortView = 0;
        _portView3D[_currentPortView].Show();

        _artTitle = GetNode<RichTextLabel>("ArtTitle/RichTextLabel");
        _artId = GetNode<RichTextLabel>("ArtId/Control/ID Number");
        _artHeight = GetNode<RichTextLabel>("ArtSize/Height");
        _artWidth = GetNode<RichTextLabel>("ArtSize/Width");
        _starRating = GetNode<StarRating>("StarRating");
        _keyboardInput = GetNode<Control>("KeyboardInput");
        _keyboardInputSize = GetNode<Control>("KeyboardInputSize");

        _animationPlayer = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _animationPlayer.Stop();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ChangeRoom") && _typing == false)
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
        if (Input.IsActionJustPressed("South RightThumb") && _starRating == GetViewport().GuiGetFocusOwner())
        {
            GetNode<StarRating>("StarRating").Increase();
            _currentFocus._rating = _starRating._rating;
        }

        if (Input.IsActionJustPressed("East RightThumb") && _starRating == GetViewport().GuiGetFocusOwner())
        {
            GetNode<StarRating>("StarRating").Decrease();
            _currentFocus._rating = _starRating._rating;
        }

        if (Input.IsActionJustPressed("South RightThumb") && GetNode("ArtId") == GetViewport().GuiGetFocusOwner() && _typing == false)
        {
            _typing = true;
            _selectedDetail = GetNode("ArtId");
            _keyboardInput.Visible = true;
            _keyboardInput.GetNode<LineEdit>("LineEdit").GrabFocus();
            _keyboardInput.GetNode<LineEdit>("LineEdit").Text = _currentFocus._id;
        }
        else if ((Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.Escape) ||
            Input.IsActionJustPressed("South RightThumb") || Input.IsActionJustPressed("East RightThumb"))
            && _typing == true && !Input.IsKeyPressed(Key.Space) && !Input.IsKeyPressed(Key.Backspace) && _selectedDetail == GetNode("ArtId") && _keyboardInput.GetNode<LineEdit>("LineEdit") == GetViewport().GuiGetFocusOwner())
        {
            _typing = false;
            _keyboardInput.Visible = false;
            GetNode<Control>("ArtId").GrabFocus();

            if (!Input.IsActionJustPressed("East RightThumb") && !Input.IsKeyPressed(Key.Escape) && Tools.ValidId(_keyboardInput.GetNode<LineEdit>("LineEdit").Text))
            {
                _currentFocus._id = _keyboardInput.GetNode<LineEdit>("LineEdit").Text.ToUpper();
            }
        }

        if (Input.IsActionJustPressed("South RightThumb") && GetNode("ArtTitle") == GetViewport().GuiGetFocusOwner() && _typing == false)
        {
            _typing = true;
            _selectedDetail = GetNode("ArtTitle");
            _keyboardInput.Visible = true;
            _keyboardInput.GetNode<LineEdit>("LineEdit").GrabFocus();
            _keyboardInput.GetNode<LineEdit>("LineEdit").Text = _currentFocus._title;
        }

        /* User can hit escape, enter, A, or B to close typing window
         * Enter/A = Save contents of keyboard input to art title
         * Escape/B = Disregard contents
         * Ignore Space and Backspace inputs
         */
        else if ((Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.Escape) ||
            Input.IsActionJustPressed("South RightThumb") || Input.IsActionJustPressed("East RightThumb"))
            && _typing == true && !Input.IsKeyPressed(Key.Space) && !Input.IsKeyPressed(Key.Backspace) && _selectedDetail == GetNode("ArtTitle") && _keyboardInput.GetNode<LineEdit>("LineEdit") == GetViewport().GuiGetFocusOwner())
        {
            _typing = false;
            _keyboardInput.Visible = false;
            GetNode<Control>("ArtTitle").GrabFocus();

            if (!Input.IsActionJustPressed("East RightThumb") && !Input.IsKeyPressed(Key.Escape))
            {
                _currentFocus._title = _keyboardInput.GetNode<LineEdit>("LineEdit").Text;
            }
        }

        if (Input.IsActionJustPressed("South RightThumb") && GetNode("ArtSize") == GetViewport().GuiGetFocusOwner() && _typing == false)
        {
            _typing = true;
            _keyboardInputSize.Visible = true;
            _keyboardInputSize.GetNode<LineEdit>("Width").GrabFocus();
            _keyboardInputSize.GetNode<LineEdit>("Width").Text = _currentFocus._width.ToString();
            _keyboardInputSize.GetNode<LineEdit>("Height").Text = _currentFocus._height.ToString();
        }
        else if ((Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.Escape) ||
          Input.IsActionJustPressed("South RightThumb") || Input.IsActionJustPressed("East RightThumb"))
          && _typing == true && !Input.IsKeyPressed(Key.Space) && !Input.IsKeyPressed(Key.Backspace) &&
          (_keyboardInputSize.GetNode<LineEdit>("Width") == GetViewport().GuiGetFocusOwner() || _keyboardInputSize.GetNode<LineEdit>("Height") == GetViewport().GuiGetFocusOwner()))
        {
            _typing = false;
            _keyboardInputSize.Visible = false;
            GetNode<Control>("ArtSize").GrabFocus();

            if (!Input.IsActionJustPressed("East RightThumb") && !Input.IsKeyPressed(Key.Escape))
            {
                _currentFocus._width = _keyboardInputSize.GetNode<LineEdit>("Width").Text.ToInt();
                _currentFocus._height = _keyboardInputSize.GetNode<LineEdit>("Height").Text.ToInt();
            }
        }

        if (Input.IsActionJustPressed("South RightThumb") && GetNode("ArtTags") == GetViewport().GuiGetFocusOwner() && _typing == false)
        {
            _typing = true;
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
        return _animationPlayer.IsPlaying() || _typing;
    }

    public void SetFocusedArt(ArtIcon currentFocus)
    {
        _portView3D[_currentPortView].ChangeArt(currentFocus.ArtTexture());
        _artTitle.Text = currentFocus._title;
        _artId.Text = currentFocus._id;
        _starRating.SetRating(currentFocus._rating);
        _artHeight.Text = currentFocus._height.ToString();
        _artWidth.Text = currentFocus._width.ToString();
    }
}