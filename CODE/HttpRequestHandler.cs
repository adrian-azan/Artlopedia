using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class HttpRequestHandler : Node
{
    private static HttpRequest _requester;
    private String _baseUrl;

    [Export]
    private bool _debugState;

    public override void _Ready()
    {
        _requester = new HttpRequest();
        AddChild(_requester);
        _requester.RequestCompleted += HttpRequestCompleted;

        var configContent = FileAccess.GetFileAsString("res://CODE/config.json");
        var config = Json.ParseString(configContent).AsGodotDictionary();
        _baseUrl = config.GetValueOrDefault("baseUrl", "").AsString();
    }

    public void GET()
    {
        Error error = _requester.Request(String.Format("{0}/art", _baseUrl), null, HttpClient.Method.Get);

        if (error != Error.Ok && _debugState)
        {
            GD.PushError(String.Format("Failure in HttpRequestHandler.GET\n\t{0}", error.ToString()));
        }
    }

    public void PUT(Array<Dictionary> allArt)
    {
        Dictionary request = new Dictionary();
        request.Add("items", allArt);

        Error error = _requester.Request(String.Format("{0}/art", _baseUrl), null, HttpClient.Method.Put, Json.Stringify(request, "\t"));

        if (error != Error.Ok && _debugState)
        {
            GD.PushError(String.Format("Failure in HttpRequestHandler.PUT\n\t{0}", error.ToString()));
        }
    }

    // Called when the HTTP request is completed.
    private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200 && _debugState)
            GD.PushError(String.Format("Request failed: {0} \n{1}", responseCode, Json.ParseString(body.GetStringFromUtf8())));
        else if (_debugState)
            GD.PushWarning(String.Format("Request Success: {0} \n{1}", responseCode, Json.ParseString(body.GetStringFromUtf8())));
    }
}