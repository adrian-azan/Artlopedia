using Godot;
using Godot.Collections;
using System;

public partial class FileManager : Node
{
    public static void Init()
    {
        if (System.IO.Directory.Exists(ArtDirectory()) == false)
            System.IO.Directory.CreateDirectory(ArtDirectory());

        if (System.IO.Directory.Exists(DetailsDirectory()) == false)
            System.IO.Directory.CreateDirectory(DetailsDirectory());

        if (System.IO.Directory.Exists(LowResolutionDirectory()) == false)
            System.IO.Directory.CreateDirectory(LowResolutionDirectory());

        string[] allImages = DirAccess.GetFilesAt(ArtDirectory());

        foreach (var artFileName in allImages)
        {
            Dictionary artDetails = new Dictionary();
            artDetails["id"] = artFileName.Split(".")[0];

            //FileName Check
            if (Tools.ValidId(artDetails["id"].ToString()) == false)
            {
                //Find a valid hex name that does not exist and rename art
                var rng = new RandomNumberGenerator();
                do
                {
                    artDetails["id"] = rng.RandiRange(0, 4095).ToString("X");
                } while (ArtExists(artDetails["id"].ToString()));
                DirAccess.RenameAbsolute(String.Format("{0}/{1}", ArtDirectory(), artFileName), String.Format("{0}/{1}.JPG", ArtDirectory(), artDetails["id"]));
            }

            //Details Check
            if (DetailsExist(artDetails["id"].ToString()) == false)
            {
                var newDetails = ArtIcon.Default();
                newDetails["id"] = artDetails["id"];
                SaveDetails(newDetails);
            }

            //LowResolutionImage Check
            if (LowResolutionExists(artDetails["id"].ToString()) == false)
            {
                var lowRezImage = Image.LoadFromFile(String.Format("{0}/{1}.JPG", ArtDirectory(), artDetails["id"]));
                lowRezImage.SaveJpg(String.Format("{0}/{1}.JPG", LowResolutionDirectory(), artDetails["id"]), .05f);
            }
        }
    }

    //---------------------------------
    // Directories
    //---------------------------------
    public static string GetExecutableDirectory()
    {
        string executedDirectoryFull = OS.GetExecutablePath();
        string rootDirectory = executedDirectoryFull.Substr(0, executedDirectoryFull.LastIndexOf('/'));

        return rootDirectory;
    }

    public static string DetailsDirectory()
    {
        return String.Format("{0}/ART/Details", GetExecutableDirectory());
    }

    public static string ArtDirectory()
    {
        return String.Format("{0}/ART", GetExecutableDirectory());
    }

    public static string LowResolutionDirectory()
    {
        return String.Format("{0}/ART/LowRes", GetExecutableDirectory());
    }

    //---------------------------------
    // Art
    //---------------------------------
    public static bool ArtExists(string artId)
    {
        return FileAccess.FileExists(String.Format("{0}/{1}.JPG", ArtDirectory(), artId));
    }

    //---------------------------------
    // Details
    //---------------------------------
    public static Dictionary LoadDetails(string artId)
    {
        using FileAccess fin = FileAccess.Open(String.Format("{0}/{1}.txt", DetailsDirectory(), artId), FileAccess.ModeFlags.Read);
        var contents = Json.ParseString(fin.GetAsText()).AsGodotDictionary();
        return contents;
    }

    public static Array<Dictionary> LoadDetails()
    {
        string[] allImages = DirAccess.GetFilesAt(FileManager.ArtDirectory());
        Array<Dictionary> contents = new Array<Dictionary>();

        foreach (var artFileName in allImages)
        {
            using FileAccess fin = FileAccess.Open(String.Format("{0}/{1}.txt", DetailsDirectory(), artFileName.Split(".")[0]), FileAccess.ModeFlags.Read);
            contents.Add(Json.ParseString(fin.GetAsText()).AsGodotDictionary());
        }

        return contents;
    }

    public static void SaveDetails(Dictionary artDetails)
    {
        using FileAccess fout = FileAccess.Open(String.Format("{0}/{1}.txt", DetailsDirectory(), artDetails["id"]), FileAccess.ModeFlags.Write);
        fout.StoreString(Json.Stringify(artDetails, "\t"));
    }

    public static void SaveDetails(Array<Dictionary> artDetails)
    {
        foreach (var detail in artDetails)
        {
            using FileAccess fout = FileAccess.Open(String.Format("{0}/{1}.txt", DetailsDirectory(), detail["id"]), FileAccess.ModeFlags.Write);
            fout.StoreString(Json.Stringify(detail, "\t"));
        }
    }

    public static bool DetailsExist(string artId)
    {
        return FileAccess.FileExists(String.Format("{0}/{1}.txt", DetailsDirectory(), artId));
    }

    //---------------------------------
    // LowResolutionImages
    //---------------------------------

    public static bool LowResolutionExists(string artId)
    {
        return FileAccess.FileExists(String.Format("{0}/{1}.JPG", LowResolutionDirectory(), artId));
    }
}