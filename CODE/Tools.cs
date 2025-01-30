using Godot;
using Godot.Collections;
using System;

public static class Tools
{
    public static Node GetRoot(Node Source)
    {
        if (Source.Owner == null)
            return Source;
        return GetRoot(Source.Owner);
    }

    public static Node GetRoot<T>(Node Source)
    {
        if (Source == null)
            return null;

        if (Source.Owner == null || Source is T)
            return Source;
        return GetRoot<T>(Source.Owner);
    }

    public static T GetChild<[MustBeVariant] T>(Node Source) where T : Node
    {
        if (Source == null)
            return null;

        Godot.Collections.Array<Node> children = Source.GetChildren();

        foreach (Node child in children)
        {
            if (child is T)
                return child as T;
        }

        foreach (Node child in children)
        {
            var result = GetChild<T>(child);
            if (result != null)
                return result;
        }

        return null;
    }

    //MustBeVariant allows Godot.Collection.Array to use Type T
    //where T : Node allows us to cast child as type T
    public static Array<T> GetChildren<[MustBeVariant] T>(Node Source) where T : Node
    {
        if (Source == null)
            return null;

        Array<T> childrenOfType = new Array<T>();
        Array<Node> children = Source.GetChildren();

        foreach (Node child in children)
        {
            if (child is T)
                childrenOfType.Add(child as T);
        }

        foreach (Node child in children)
        {
            childrenOfType += GetChildren<T>(child);
        }

        return childrenOfType;
    }

    public static float DegToRad(float degrees)
    {
        return degrees * Mathf.Pi / 180;
    }

    public static float distanceFromCenter(int x, int y, int width, int height)
    {
        var centerX = width / 2;
        var centerY = height / 2;

        var distanceX = Mathf.Abs(centerX - x);
        var distanceY = Mathf.Abs(centerY - y);

        return Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));
    }

    public static float distanceFromPoint(int x, int y, int pointX, int pointY)
    {
        var distanceX = Mathf.Abs(pointX - x);
        var distanceY = Mathf.Abs(pointY - y);

        return Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));
    }

    //File Tools
    public static bool ValidId(string id)
    {
        if (id == null || id.Length > 3) return false;
        if (id.IsValidHexNumber() == false) return false;
        if (FileAccess.FileExists(String.Format("res://ART/Your Art Here/Details/{0}.txt", id))) return false;
        return true;
    }
}