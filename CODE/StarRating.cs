using Godot;
using System;
using System.Linq;

public partial class StarRating : DetailsIcon
{
    public float _rating;

    public override void _Ready()
    {
        _rating = 2.5f;
        UpdateGui();
    }

    public void UpdateGui()
    {
        var tempRating = _rating;
        var starSprites = GetChildren();

        foreach (Sprite2D filledStar in starSprites.Take((int)_rating))
        {
            filledStar.Texture = ResourceLoader.Load<Texture2D>("res://ART/UI/FullStar.png");
        }

        foreach (Sprite2D emptyStar in starSprites.TakeLast(5 - ((int)_rating)))
        {
            emptyStar.Texture = ResourceLoader.Load<Texture2D>("res://ART/UI/EmptyStar.png");
        }

        if (_rating != (int)_rating)
        {
            (starSprites[(int)_rating] as Sprite2D).Texture = ResourceLoader.Load<Texture2D>("res://ART/UI/HalfStar.png");
        }
    }

    public void SetRating(float rating)
    {
        if (rating < .5f) rating = .5f;
        if (rating > 5) rating = 5;
        _rating = rating;
        UpdateGui();
    }

    public void Increase()
    {
        if (_rating >= 5) { _rating = 5; return; }

        _rating += .5f;
        UpdateGui();
    }

    public void Decrease()
    {
        if (_rating <= .5f) { _rating = .5f; return; }

        _rating -= .5f;
        UpdateGui();
    }
}