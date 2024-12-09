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
        _httpRequestHandler.RequestCompleted += ProcessCompletedRequest;
        _httpRequestHandler.GET(); //Retrieve Art details on start of program

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
                requestItems.Add(art.Serialize());
            }

            _httpRequestHandler.PUT(requestItems);
        }

        if (Input.IsActionJustPressed("DownloadArt"))
        {
            _httpRequestHandler.GET();
        }
    }

    public void ProcessCompletedRequest(long result, long responseCode, string[] headers, byte[] body)
    {
        if (_httpRequestHandler._lastRequest == HttpRequestHandler.RequestTypes.GET_ALL_ART)
        {
            var requestResponse = Json.ParseString(body.GetStringFromUtf8()).AsGodotDictionary();

            var allRemoteArt_Unformatted = requestResponse["body"].AsGodotArray();
            Dictionary<string, Dictionary> allArtDetails = new Dictionary<string, Dictionary>();

            //Convert list of artDetails into a dictionary of artDetails
            // {ID: ArtDetails} will allow iconCollection to easily find artIcon to configure
            foreach (var artDetails in allRemoteArt_Unformatted)
            {
                Dictionary artDetailsDictionary = Json.ParseString(artDetails.AsString()).AsGodotDictionary();
                allArtDetails.Add(artDetailsDictionary["id"].AsString(), artDetailsDictionary);
            }

            _iconCollection.AllArt(allArtDetails);
        }

        _httpRequestHandler._lastRequest = HttpRequestHandler.RequestTypes.NONE;
    }
}