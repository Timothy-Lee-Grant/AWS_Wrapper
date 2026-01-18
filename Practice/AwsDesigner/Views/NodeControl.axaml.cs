using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AwsDesigner.ViewModels;
using Avalonia.VisualTree;

namespace AwsDesigner.Views;

public partial class NodeControl : UserControl
{
    private bool _isDragging;
    private double _startX, _startY;

    public NodeControl()
    {
        InitializeComponent();
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
        var p = e.GetPosition(this);
        _startX = p.X;
        _startY = p.Y;
        e.Pointer.Capture(this);

        if (DataContext is NodeViewModel vm && this.FindAncestorOfType<Window>() is Window w && w.DataContext is MainWindowViewModel mw)
        {
            mw.Workspace.SelectedNode = vm;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;
        if (DataContext is NodeViewModel vm && this.Parent is Avalonia.Controls.Canvas canvas)
        {
            var p = e.GetPosition(canvas);
            vm.X = p.X - _startX;
            vm.Y = p.Y - _startY;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
    }
}
