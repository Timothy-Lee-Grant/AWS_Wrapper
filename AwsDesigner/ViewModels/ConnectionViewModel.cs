using Avalonia;
using Avalonia.Media;
using System.ComponentModel;

namespace AwsDesigner.ViewModels;

public class ConnectionViewModel : ViewModelBase
{
    private NodeViewModel? _source;
    private NodeViewModel? _target;

    public NodeViewModel? Source
    {
        get => _source;
        set
        {
            if (_source == value) return;
            if (_source != null) _source.PropertyChanged -= Node_PropertyChanged;
            _source = value;
            if (_source != null) _source.PropertyChanged += Node_PropertyChanged;
            OnPropertyChanged(nameof(PathGeometry));
        }
    }

    public NodeViewModel? Target
    {
        get => _target;
        set
        {
            if (_target == value) return;
            if (_target != null) _target.PropertyChanged -= Node_PropertyChanged;
            _target = value;
            if (_target != null) _target.PropertyChanged += Node_PropertyChanged;
            OnPropertyChanged(nameof(PathGeometry));
        }
    }

    private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NodeViewModel.X) || e.PropertyName == nameof(NodeViewModel.Y))
            OnPropertyChanged(nameof(PathGeometry));
    }

    public PathGeometry PathGeometry
    {
        get
        {
            if (Source == null || Target == null) return new PathGeometry();
            var sx = Source.X + Source.Width / 2;
            var sy = Source.Y + Source.Height / 2;
            var tx = Target.X + Target.Width / 2;
            var ty = Target.Y + Target.Height / 2;

            var pg = new PathGeometry();
            var fig = new PathFigure { StartPoint = new Point(sx, sy) };
            var midX = (sx + tx) / 2;
            fig.Segments.Add(new BezierSegment(new Point(midX, sy), new Point(midX, ty), new Point(tx, ty)));
            pg.Figures.Add(fig);
            return pg;
        }
    }
}
