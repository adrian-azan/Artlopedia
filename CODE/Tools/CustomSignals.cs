using Godot;
using System;

public partial class CustomSignals : Node
{
    public static CustomSignals _Instance;

    public override void _Ready()
    {
        base._Ready();

        _Instance = this;
    }

    [Signal]
    public delegate void Rotate3DArtEventHandler(float rotationZ);

    [Signal]
    public delegate void ChangeArtEventHandler(ArtIcon newArtTexture);

    [Signal]
    public delegate void SaveArtEventHandler();

}