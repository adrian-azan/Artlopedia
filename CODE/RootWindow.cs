using Godot;
using Godot.Collections;
using System;

public partial class RootWindow : Node2D
{
    private IconCollection _iconCollection;
    private RightPanel _rightPanel;
    private HttpRequestHandler _httpRequestHandler;

    private enum State
    {
        Icon,
        Details,
        ArtFocused
    };

    private State _state;

    public override void _Ready()
    {
        _iconCollection = GetNode<IconCollection>("IconCollection");
        _rightPanel = GetNode<RightPanel>("RightPanel");
        _httpRequestHandler = GetNode<HttpRequestHandler>("HttpRequestHandler");
        _state = State.Icon;
    }

    public override void _Process(double delta)
    {
        _rightPanel.SetFocusedArt(_iconCollection.FocusedArtIcon());

        if (_state == State.Icon)
        {
            _iconCollection._Control(delta);

            if (Input.IsActionJustPressed("South RightThumb") && _rightPanel.Busy() == false)
            {
                _rightPanel.Focus3DView();
                _state = State.ArtFocused;
            }
        }
        else if (_state == State.ArtFocused && _rightPanel.Busy() == false)
        {
            if (Input.IsActionJustPressed("East RightThumb"))
            {
                _rightPanel.UnFocus3DView();
                _state = State.Icon;
            }
        }

        if (Input.IsActionJustPressed("UploadArt"))
        {
            Array<Dictionary> requestItems = new Array<Dictionary>();
            foreach (var art in _iconCollection.AllArt())
            {
                requestItems.Add(art.ToJson());
            }

            _httpRequestHandler.PUT(requestItems);
        }

        if (Input.IsActionJustPressed("DownloadArt"))
        {
            _httpRequestHandler.GET();
        }
    }
}