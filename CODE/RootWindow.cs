using Godot;
using System;

public partial class RootWindow : Node2D
{
    private IconCollection _iconCollection;
    private RightPanel _rightPanel;

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
        _state = State.Icon;
    }

    public override void _Process(double delta)
    {
        _rightPanel.SetFocusedArt(_iconCollection.FocusedArtIcon());

        if (_state == State.Icon)
        {
            _iconCollection._Control(delta);

            if (Input.IsActionJustPressed("South RightThumb"))
            {
                _rightPanel.Focus3DView();
            }
        }
        else if (_state == State.Details)
        {
        }
    }
}