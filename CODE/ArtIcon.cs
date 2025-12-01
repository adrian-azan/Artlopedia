using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class ArtIcon : Control
{
    private AspectRatioContainer _container;
    private TextureRect _background;
    private TextureRect _art;

    private Texture2D _artHighRes;

    private Texture2D _highlighted;
    private Texture2D _normal;

    public string _id;
    public string _title;
    public float _rating;
    public string[] _tags;
    public string _locationPurchased;
    public float _width;
    public float _height;
    public float _orientation2D;
    public float _orientation3D;

    public Task LOADING;

    public override void _Ready()
    {
        _background = GetNode<TextureRect>("Background");
        _art = GetNode<TextureRect>("AspectRatioContainer/Art");
        _container = GetNode<AspectRatioContainer>("AspectRatioContainer");

        _highlighted = ResourceLoader.Load("res://ART/UI/ArtBackground Highlighted.png") as Texture2D;
        _normal = ResourceLoader.Load("res://ART/UI/ArtBackground.png") as Texture2D;
    }

    public async void LoadLowResolution()
    {
        if (_art.Texture != null) return;

        ImageTexture artLowRes = null;
        LOADING = Task.Run(() =>
        {
            Logging.PrintInfo(Logging.Category_Data_Management, "Loading Low Rez", "LowRez");

            
            var image = Image.LoadFromFile(String.Format("{0}/{1}.JPG", FileManager.LowResolutionDirectory(), _id));
            image.ShrinkX2();
            image.ShrinkX2();
            image.Compress(Image.CompressMode.S3Tc);
            artLowRes = ImageTexture.CreateFromImage(image);
            
            Logging.PrintInfo(Logging.Category_Data_Management, "Loaded Low Rez", "LowRez");
        });
        await LOADING;

        _art.Texture = artLowRes;
    }

    public async void LoadHighResolution()
    {
        //TODO: HighRes art should only be loaded for icons near selection and should be removed from memory.
        if (_artHighRes != null)
            return;
        
        await Task.Run(() =>
      {
          Logging.PrintInfo(Logging.Category_Data_Management, "Loading High Rez", "HighLow");

          var image = Image.LoadFromFile(String.Format("{0}/{1}.JPG", FileManager.ArtDirectory(), _id));
          image.ShrinkX2();
          image.ShrinkX2();
          image.Compress(Image.CompressMode.Bptc);
          _artHighRes = ImageTexture.CreateFromImage(image);
          
          Logging.PrintInfo(Logging.Category_Data_Management, "Loaded High Rez", "HighLow");
      });

    }

    public void Clear()
    {
        _art.Texture = null;
        _artHighRes = null;
    }

    public Texture2D ArtTexture()
    {
        return _artHighRes;
    }

    public void RotateClockwise()
    {
        _container.RotationDegrees += 90;
    }

    public void RotateCounterClockwise()
    {
        _container.RotationDegrees -= 90;
    }

    public void Highlight()
    {
        _background.Texture = _highlighted;
    }

    public void UnHighlight()
    {
        _background.Texture = _normal;
    }

    public void DebugHighlight()
    {
        CreateTween().TweenProperty(GetNode<Sprite2D>("Sprite2D"), "visible", true, 0);
        CreateTween().TweenProperty(GetNode<Sprite2D>("Sprite2D"), "visible", false, 2);
    }

    public void Deserialize(Dictionary artDetails, bool withImage = false)
    {
        _height = (float)artDetails["dimensions"].AsGodotDictionary()["height"];
        _width = (float)artDetails["dimensions"].AsGodotDictionary()["width"];

        _orientation2D = (float)artDetails["orientation"].AsGodotDictionary()["2D"];
        _orientation3D = (float)artDetails["orientation"].AsGodotDictionary()["3D"];

        _container.RotationDegrees = _orientation2D;

        _id = artDetails["id"].AsString();
        _locationPurchased = artDetails["locationPurchased"].AsString();
        _rating = (float)artDetails["rating"];
        _tags = artDetails["tags"].AsStringArray();
        _title = artDetails["title"].AsString();

        if (withImage)
        {
            LoadLowResolution();
        }
    }

    public Dictionary Serialize()
    {
        Dictionary output = new Dictionary();
        Dictionary dimensions = new Dictionary();
        dimensions.Add("width", _width);
        dimensions.Add("height", _height);
        Dictionary orientation = new Dictionary();
        orientation.Add("2D", _container.RotationDegrees);
        orientation.Add("3D", _orientation3D);

        output.Add("title", _title);
        output.Add("id", _id);
        output.Add("rating", _rating);
        output.Add("tags", _tags);
        output.Add("locationPurchased", _locationPurchased);
        output.Add("dimensions", dimensions);
        output.Add("orientation", orientation);

        return output;
    }

    public static Dictionary Default()
    {
        Dictionary defaultDetails = new Dictionary();
        Dictionary dimensionsDefault = new Dictionary();
        dimensionsDefault.Add("width", 0);
        dimensionsDefault.Add("height", 0);
        Dictionary orientationDefault = new Dictionary();
        orientationDefault.Add("2D", 0);
        orientationDefault.Add("3D", 0);

        defaultDetails.Add("title", "");
        defaultDetails.Add("id", "00");
        defaultDetails.Add("rating", 2.5);
        defaultDetails.Add("tags", new string[0]);
        defaultDetails.Add("locationPurchased", "");
        defaultDetails.Add("dimensions", dimensionsDefault);
        defaultDetails.Add("orientation", orientationDefault);

        return defaultDetails;
    }
}