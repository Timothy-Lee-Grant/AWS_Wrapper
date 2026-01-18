using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AwsDesigner.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

    public CommandsViewModel Commands { get; }

    public NodeViewModel? SelectedNode { get; set; }

    private int _nextId = 1;
    private NodeViewModel? _connectFrom;

    public WorkspaceViewModel()
    {
        Commands = new CommandsViewModel(this);
    }

    public NodeViewModel AddNode(string resourceType, double x = 100, double y = 100)
    {
        var node = new NodeViewModel
        {
            Id = _nextId++,
            ResourceType = resourceType,
            Title = resourceType,
            X = x,
            Y = y,
            Width = 140,
            Height = 72,
        };
        Nodes.Add(node);
        return node;
    }

    public void StartConnect()
    {
        // toggle simple connect mode — choose first node then second in inspector (or via UI later)
        _connectFrom = null;
    }

    public void TryConnect(NodeViewModel node)
    {
        if (_connectFrom == null)
        {
            _connectFrom = node;
            return;
        }

        if (_connectFrom != node)
        {
            if (!Connections.Any(c => (c.Source == _connectFrom && c.Target == node) || (c.Source == node && c.Target == _connectFrom)))
                Connections.Add(new ConnectionViewModel { Source = _connectFrom, Target = node });
        }

        _connectFrom = null;
    }

    public async Task DeployAsync()
    {
        // Dummy deploy — replace with AWS SDK / CloudFormation integration later.
        await Task.Delay(300);
    }
}
