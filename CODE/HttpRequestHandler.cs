using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class HttpRequestHandler : HttpRequest
{
    public enum RequestTypes
    {
        NONE,
        GET_ALL_ART,
        PUT_ALL_ART
    };

    public RequestTypes _lastRequest;

    private string _baseUrl;

    [Export]
    private bool _debugState;

    public override void _Ready()
    {
        if (_debugState)
            RequestCompleted += DEBUG_HttpRequestCompleted;

        var configContent = FileAccess.GetFileAsString("res://CODE/config.json");
        var config = Json.ParseString(configContent).AsGodotDictionary();
        _baseUrl = config.GetValueOrDefault("baseUrl", "").AsString();
    }

    public void GET()
    {
        if (_lastRequest != RequestTypes.NONE)
            return;

        Error error = Request(String.Format("{0}/art", _baseUrl), null, HttpClient.Method.Get);

        if (error != Error.Ok && _debugState)
        {
            GD.PushError(String.Format("Failure in HttpRequestHandler.GET\n\t{0}", error.ToString()));
            return;
        }

        _lastRequest = RequestTypes.GET_ALL_ART;
    }

    public void PUT(Array<Dictionary> allArt)
    {
        if (_lastRequest != RequestTypes.NONE)
            return;

        Dictionary request = new Dictionary();
        request.Add("items", allArt);

        Error error = Request(String.Format("{0}/art", _baseUrl), null, HttpClient.Method.Put, Json.Stringify(request, "\t"));

        if (error != Error.Ok && _debugState)
        {
            GD.PushError(String.Format("Failure in HttpRequestHandler.PUT\n\t{0}", error.ToString()));
            return;
        }

        _lastRequest = RequestTypes.PUT_ALL_ART;
    }

    private void DEBUG_HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
            GD.PushError(String.Format("Request failed: {0} \n{1}", responseCode, Json.ParseString(body.GetStringFromUtf8())));
        else
            GD.PushWarning(String.Format("Request Success: {0} \n{1}", responseCode, Json.ParseString(body.GetStringFromUtf8())));
    }
}