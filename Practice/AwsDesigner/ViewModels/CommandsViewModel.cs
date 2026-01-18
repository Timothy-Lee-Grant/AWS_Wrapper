using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AwsDesigner.ViewModels;

public class CommandsViewModel
{
    private readonly WorkspaceViewModel _workspace;

    public ICommand AddEc2 { get; }
    public ICommand AddS3 { get; }
    public ICommand AddRds { get; }
    public ICommand StartConnect { get; }
    public ICommand Deploy { get; }

    public CommandsViewModel(WorkspaceViewModel workspace)
    {
        _workspace = workspace;
        AddEc2 = new RelayCommand(() => _workspace.AddNode("EC2", 100, 100));
        AddS3 = new RelayCommand(() => _workspace.AddNode("S3", 260, 120));
        AddRds = new RelayCommand(() => _workspace.AddNode("RDS", 420, 200));
        StartConnect = new RelayCommand(() => _workspace.StartConnect());
        Deploy = new AsyncRelayCommand(async () => await _workspace.DeployAsync());
    }
}
