using Amazon.Runtime.Internal;
using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class RootWindow : Node2D
{
    private IconCollection _iconCollection;
    private RightPanel _rightPanel;
    private HttpRequestHandler _httpRequestHandler;

    public enum SavePreference
    {
        Local,
        Remote,
        Both
    };

    private enum State
    {
        Icon,
        Details,
        ArtFocused
    };

    private State _state;

    [Export]
    public SavePreference _savePreference = SavePreference.Local;

    public override void _Ready()
    {
        _iconCollection = GetNode<IconCollection>("IconCollection");
        _rightPanel = GetNode<RightPanel>("RightPanel");

        _httpRequestHandler = GetNode<HttpRequestHandler>("HttpRequestHandler");
        _httpRequestHandler.RequestCompleted += ProcessCompletedRequest;

        SaveAllArt();
        LoadAllArt();

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
        else if (_state == State.ArtFocused && _rightPanel.Busy() == false && Input.IsActionJustPressed("East RightThumb"))
        {
            _rightPanel.UnFocus3DView();
            _state = State.Icon;
        }

        if (_state == State.Icon && Input.IsActionJustPressed("West RightThumb") && _rightPanel.Busy() == false)
        {
            GetNode<Control>("RightPanel/ArtTitle").GrabFocus();
            _state = State.Details;
        }
        else if (_state == State.Details && Input.IsActionJustPressed("West RightThumb") && _rightPanel.Busy() == false)
        {
            GetViewport().GuiReleaseFocus();
            _state = State.Icon;
        }

        if (_state == State.Details)
        {
            var guiFocus = GetViewport().GuiGetFocusOwner() as Control;
            if (guiFocus != null)
            {
                var highlightable = guiFocus as DetailsIcon;
                if (highlightable != null)
                    highlightable.Highlight();
                _rightPanel.ProcessInput(_iconCollection.FocusedArtIcon());
            }
        }

        //SYNCING CONTROLS
        if (Input.IsActionJustPressed("UploadArt"))
        {
            SaveAllArt();
        }

        if (Input.IsActionJustPressed("DownloadArt"))
        {
            LoadAllArt();
        }
    }

    public void SaveAllArt()
    {
        Array<Dictionary> artIcons = new Array<Dictionary>();
        foreach (var art in _iconCollection.AllArt())
        {
            artIcons.Add(art.Serialize());
        }

        if (_savePreference == SavePreference.Remote || _savePreference == SavePreference.Both)
            _httpRequestHandler.PUT(artIcons);
        else if (_savePreference == SavePreference.Local || _savePreference == SavePreference.Both)
            _iconCollection.SAVE(artIcons);
    }

    public void LoadAllArt()
    {
        if (_savePreference == SavePreference.Remote || _savePreference == SavePreference.Both)
            _httpRequestHandler.GET();
        else if (_savePreference == SavePreference.Local || _savePreference == SavePreference.Both)
            _iconCollection.LOAD();
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