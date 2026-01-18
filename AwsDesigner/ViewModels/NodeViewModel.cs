namespace AwsDesigner.ViewModels;

public class NodeViewModel : ViewModelBase
{
    private double _x;
    private double _y;

    public int Id { get; set; }
    public string ResourceType { get; set; } = "Resource";
    public string Title { get; set; } = "Resource";
    public double Width { get; set; } = 140;
    public double Height { get; set; } = 72;

    public double X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }

    public double Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
}
