# Concepts Lecture — Building the AWS Designer (Avalonia + MVVM)

This is a long-form lecture-style document covering the architecture, technologies, and implementation patterns used by the `AwsDesigner` project. It explains the key concepts, why they were chosen, and demonstrates how the code in this repository implements them. Expect code snippets, small diagrams, debugging tips, and extension guidance.

Target audience: mid-level software engineers unfamiliar with Avalonia or desktop MVVM but comfortable with C# and modern .NET.

---

Table of contents
- Introduction & goals
- Technology stack and dependencies
- MVVM pattern — mapping concepts to code
- Avalonia and XAML fundamentals used in the project
- Data binding and change notifications (ObservableObject)
- ItemsControl, Canvas layout, and layering
- Implementing drag & drop with pointer events
- Drawing connections: PathGeometry & Bezier curves
- Commands, Async commands, and `ICommand`
- Project structure: files to know and responsibilities
- Debugging XAML and common build issues
- Testing suggestions (unit testing ViewModels)
- Extensibility & next steps (AWS integration, save/load, undo)
- Appendix: key code excerpts from the project

---

Introduction & goals
---------------------

The `AwsDesigner` prototype demonstrates a desktop GUI that lets users place AWS resources (nodes) on a canvas, drag them around, and draw connections (dependencies/flows). The primary goals of the architecture are:

- Clear separation of UI (Views) and logic/state (ViewModels).
- Live updating of visual elements (connections) as nodes move.
- A small, testable core that can later be extended to produce CloudFormation templates or call AWS SDK APIs.

We'll build using MVVM (Model-View-ViewModel) and Avalonia UI for cross-platform desktop rendering.

Technology stack and dependencies
--------------------------------

Primary packages (from `AwsDesigner.csproj`):

- Avalonia: cross-platform XAML UI framework (controls, XAML loader).
- Avalonia.ReactiveUI: ReactiveUI integrations (optional; used for AppBuilder wiring).
- Avalonia.Desktop: platform extensions (adds UsePlatformDetect and desktop-specific services).
- Avalonia.Themes.Fluent: Fluent theme for the app visuals.
- CommunityToolkit.Mvvm: lightweight MVVM helpers (`ObservableObject`, `RelayCommand`, `AsyncRelayCommand`).

These provide a familiar WPF-like XAML experience but targeted at .NET Core/.NET 5+ cross-platform apps.

MVVM pattern — mapping concepts to code
--------------------------------------

MVVM is a UI architectural pattern separating concerns:

- Model: domain objects (not much in the prototype yet; future `AwsResource` classes).
- View: XAML + minimal code-behind (rendering and pointer capture logic). Examples: `MainWindow.axaml`, `NodeControl.axaml`.
- ViewModel: state and commands. Examples: `WorkspaceViewModel`, `NodeViewModel`, `ConnectionViewModel`, `MainWindowViewModel`.

Why MVVM here?

- Makes logic testable (ViewModels don't reference UI controls directly).
- Keeps UI declarative (XAML) and driven by data.

Mapping example (simplified):

View: `MainWindow.axaml`

```xml
<ItemsControl ItemsSource="{Binding Workspace.Nodes}">
  <ItemsControl.ItemsPanel>
    <ItemsPanelTemplate>
      <Canvas />
    </ItemsPanelTemplate>
  </ItemsControl.ItemsPanel>
  <ItemsControl.ItemTemplate>
    <DataTemplate>
      <views:NodeControl Canvas.Left="{Binding X}" Canvas.Top="{Binding Y}"/>
    </DataTemplate>
  </ItemsControl.ItemTemplate>
</ItemsControl>
```

ViewModel: `WorkspaceViewModel` exposes `Nodes` (ObservableCollection<NodeViewModel>), which drives the ItemsControl. Each `NodeViewModel` contains `X/Y` which the view binds as `Canvas.Left/Top`.

Avalonia and XAML fundamentals used in the project
-------------------------------------------------

Avalonia uses XAML (XML dialect) for declaring UI. Key points used here:

- DataTemplates and `ItemsControl` render collections.
- `Canvas` allows absolute positioning using attached properties `Canvas.Left` and `Canvas.Top`.
- Controls support `PointerPressed/PointerMoved/PointerReleased` events for mouse/touch interaction.
- `Path` + `PathGeometry` render arbitrary vector paths (used for connections).

Important syntax examples are in `Views/MainWindow.axaml` and template files.

Data binding and change notifications (ObservableObject)
-----------------------------------------------------

Bindings in XAML connect view properties to ViewModel properties. For binding to work reactively, ViewModels must notify the UI when properties change.

We use `CommunityToolkit.Mvvm.ComponentModel.ObservableObject` (base class in `ViewModelBase`) which implements `INotifyPropertyChanged` and provides `SetProperty` helper.

Example `NodeViewModel`:

```csharp
public class NodeViewModel : ViewModelBase
{
    private double _x;
    public double X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }

    private double _y;
    public double Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
}
```

`SetProperty` raises `PropertyChanged` when the value changes — the binding engine updates the UI accordingly.

ItemsControl, Canvas layout, and layering
----------------------------------------

The canvas-based editor uses layered `ItemsControl`s to render connections beneath nodes.

Simplified structure:

- `ItemsControl` (Connections) — renders `ConnectionView` elements on a Canvas.
- `ItemsControl` (Nodes) — renders `NodeControl` elements on another Canvas.

Because each `ItemsControl`'s `ItemsPanel` is a `Canvas`, nodes can be positioned absolutely via `Canvas.Left` and `Canvas.Top`.

Layering diagram (top-down):

```
| Window                                 |
|----------------------------------------|
| Toolbox | Canvas (Connections + Nodes) | Inspector |
|         |  - Connections (Paths)        |           |
|         |  - Nodes (NodeControl)        |           |
------------------------------------------
```

Implementing drag & drop with pointer events
--------------------------------------------

We implement dragging directly in the node control's code-behind using pointer events and capturing the pointer. The key steps are:

1. On `PointerPressed` record the pointer offset into the control and `Capture` the pointer.
2. On `PointerMoved`, if dragging, compute new canvas coordinates (pointer position relative to the canvas) and set `NodeViewModel.X/Y`.
3. On `PointerReleased`, clear dragging state and release pointer capture.

Snippet from `Views/NodeControl.axaml.cs`:

```csharp
private bool _isDragging;
private double _startX, _startY;

private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
{
    _isDragging = true;
    var p = e.GetPosition(this);
    _startX = p.X; _startY = p.Y;
    e.Pointer.Capture(this);
    // update selection for inspector
    if (DataContext is NodeViewModel vm && FindAncestorOfType<Window>()?.DataContext is MainWindowViewModel mw)
        mw.Workspace.SelectedNode = vm;
}

private void OnPointerMoved(object? sender, PointerEventArgs e)
{
    if (!_isDragging) return;
    if (DataContext is NodeViewModel vm && Parent is Canvas canvas)
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
```

Notes and edge cases:

- Coordinate spaces: `GetPosition()` accepts a reference control. We want canvas coordinates to compute absolute position.
- Pointer capture ensures we continue receiving pointer events even if the pointer leaves the control bounds while dragging.
- Consider adding bounds checks to prevent dragging nodes off the canvas (or implement auto-scrolling).

Drawing connections: PathGeometry & Bezier curves
-----------------------------------------------

Connections are rendered with a `Path` element whose `Data` is bound to `ConnectionViewModel.PathGeometry`.

We compute a simple cubic Bezier from the center of source node to center of target node. The control points are chosen to create a smooth curve by using an intermediate X coordinate.

Key snippet from `ConnectionViewModel`:

```csharp
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
        var bez = new BezierSegment
        {
            Point1 = new Point(midX, sy),
            Point2 = new Point(midX, ty),
            Point3 = new Point(tx, ty)
        };
        fig.Segments.Add(bez);
        pg.Figures.Add(fig);
        return pg;
    }
}
```

The `ConnectionViewModel` also subscribes to `PropertyChanged` on both source and target so it raises `PropertyChanged(nameof(PathGeometry))` when node positions change, causing the UI to re-evaluate the binding and visually update the path.

Commands, Async commands, and `ICommand`
---------------------------------------

Commands are the primary way to expose user actions from the UI to a ViewModel. The UI binds a `Button`'s `Command` property to an `ICommand` on a ViewModel.

We use `RelayCommand` and `AsyncRelayCommand` from `CommunityToolkit.Mvvm.Input` to create simple command instances.

Example `CommandsViewModel` that the toolbar binds to:

```csharp
public class CommandsViewModel
{
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
```

`AsyncRelayCommand` correctly handles async/await and can disable re-entry or propagate exceptions if desired.

Project structure: files to know and responsibilities
---------------------------------------------------

- `Program.cs` — the application entry point and Avalonia app builder.
- `App.axaml` / `App.axaml.cs` — application resources and initialization (assigns `MainWindow`).
- `Views/` — XAML views (UI definitions) and small code-behind files for pointer handling.
- `ViewModels/` — all viewmodels and small command holder.
- `Docs/` — documentation. (This lecture file is stored at `Docs/Concepts_Lecture.md`.)

Debugging XAML and common build issues
-------------------------------------

Common errors you may see and how we resolved them during prototype:

- "Unable to resolve type FluentTheme": missing `Avalonia.Themes.Fluent` package in the csproj.
- "App not found": ensure `Program.cs` includes the project's root namespace so `App` is discoverable.
- "UsePlatformDetect not found": add `Avalonia.Desktop` package.
- XAML property errors (e.g., `Padding` on `StackPanel`): check the control API; use `Margin` on `StackPanel`.
- Binding to `Items` vs `ItemsSource`: `ItemsSource` is used to supply a collection.

Useful workflow when encountering XAML build errors:

1. Read the build/XAML parse error — it usually points to file and line.
2. Check the XAML control and ensure the property exists (use Avalonia docs or intellisense).
3. If type resolution fails, verify package references and correct XML namespaces.

Testing suggestions (unit testing ViewModels)
-------------------------------------------

ViewModels are intentionally testable because they don't depend on UI controls. Example tests:

- `WorkspaceViewModel.AddNode` should add a node with the expected `ResourceType`, `X` and `Y`.
- `WorkspaceViewModel.TryConnect` should create a `ConnectionViewModel` between two nodes and avoid duplicates.
- `ConnectionViewModel.PathGeometry` should produce deterministic results for known node coordinates.

You can use xUnit or NUnit. Example (pseudocode):

```csharp
[Fact]
public void AddNode_CreatesNode()
{
    var ws = new WorkspaceViewModel();
    var node = ws.AddNode("S3", 10, 20);
    Assert.Contains(node, ws.Nodes);
    Assert.Equal(10, node.X);
    Assert.Equal(20, node.Y);
}
```

Extensibility & next steps (AWS integration, save/load, undo)
-----------------------------------------------------------

This architecture was selected to make the following tasks straightforward:

1. Add an `IAwsDeploymentService` abstraction and a concrete implementation using AWS SDK (or CloudFormation generation). Call this from `WorkspaceViewModel.DeployAsync()`.
2. Add serialization for diagrams: serialize `WorkspaceViewModel.Nodes` and `Connections` to JSON to support save/load.
3. Add an undo/redo stack: record commands (node move, add/remove node, connect/disconnect) and implement reversal logic.
4. Add authentication and settings UI for AWS credentials or profile selection.

Appendix: key code excerpts from the project
-------------------------------------------

`Program.cs` (entry point):

```csharp
using Avalonia;
using Avalonia.ReactiveUI;
using AwsDesigner;

internal class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
```

`App.axaml` (resources):

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="AwsDesigner.App">
  <Application.Styles>
    <FluentTheme />
  </Application.Styles>
</Application>
```

`MainWindow` structure (core canvas, toolbox, inspector): see `Views/MainWindow.axaml`.

`WorkspaceViewModel` (core state & actions):

```csharp
public class WorkspaceViewModel : ViewModelBase
{
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
    public CommandsViewModel Commands { get; }
    public NodeViewModel? SelectedNode { get; set; }

    public NodeViewModel AddNode(string resourceType, double x = 100, double y = 100) { ... }
    public void TryConnect(NodeViewModel node) { ... }
    public async Task DeployAsync() { /* TODO: call AWS */ }
}
```

`NodeControl` dragging (code-behind): see earlier pointer event snippet.

`ConnectionViewModel` path computation & subscription:

```csharp
public NodeViewModel? Source { get; set; }
public NodeViewModel? Target { get; set; }

// On Source/Target set, we subscribe to PropertyChanged so PathGeometry is recalculated when X/Y change.
```

Closing notes
-------------

This lecture has covered the design and implementation concepts used in the `AwsDesigner` prototype, with code examples and practical notes for debugging and extension. If you'd like, I can follow up with any of these hands-on tasks:

- Implement `IAwsDeploymentService` + mock and show a sample CloudFormation JSON generator.
- Add JSON save/load for the workspace and a simple `File -> Save/Load` UI.
- Implement an undo/redo command history for node operations.

Tell me which follow-up you'd like and I'll implement it next.
