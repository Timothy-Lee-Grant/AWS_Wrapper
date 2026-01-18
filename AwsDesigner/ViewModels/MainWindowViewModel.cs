namespace AwsDesigner.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public WorkspaceViewModel Workspace { get; } = new WorkspaceViewModel();

    public MainWindowViewModel()
    {
        // seed demo nodes
        Workspace.AddNode("EC2", 80, 80);
        Workspace.AddNode("S3", 260, 120);
    }
}
