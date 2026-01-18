# AWS Designer — Onboarding & Architecture

This document explains the architecture and design of the `AwsDesigner` Avalonia/.NET 9 desktop application scaffold generated in this workspace. It's intended as a concise, practical onboarding for a mid-level software engineer so you can explore, run, maintain, and extend the app.

**Status:** Prototype UI with MVVM structure, draggable nodes, visual connections, and a dummy deploy stub (no AWS integration yet).

**Quick Start**
- **Build:** `dotnet build AwsDesigner` (or run from the `AwsDesigner` directory)
- **Run:**

```bash
cd AwsDesigner
dotnet build
dotnet run
```

If missing packages appear on first run, `dotnet restore` will run automatically during `dotnet build`.

**Project Layout**
- **Project root:** [AwsDesigner/AwsDesigner.csproj](AwsDesigner/AwsDesigner.csproj): app manifest and NuGet references.
- **Entry point:** [AwsDesigner/Program.cs](AwsDesigner/Program.cs)
- **Application:** [AwsDesigner/App.axaml](AwsDesigner/App.axaml) and [AwsDesigner/App.axaml.cs](AwsDesigner/App.axaml.cs)
- **Views:** [AwsDesigner/Views/MainWindow.axaml](AwsDesigner/Views/MainWindow.axaml), [AwsDesigner/Views/NodeControl.axaml](AwsDesigner/Views/NodeControl.axaml), [AwsDesigner/Views/ConnectionView.axaml](AwsDesigner/Views/ConnectionView.axaml)
- **ViewModels:** [AwsDesigner/ViewModels/MainWindowViewModel.cs](AwsDesigner/ViewModels/MainWindowViewModel.cs), [AwsDesigner/ViewModels/WorkspaceViewModel.cs](AwsDesigner/ViewModels/WorkspaceViewModel.cs), [AwsDesigner/ViewModels/NodeViewModel.cs](AwsDesigner/ViewModels/NodeViewModel.cs), [AwsDesigner/ViewModels/ConnectionViewModel.cs](AwsDesigner/ViewModels/ConnectionViewModel.cs), [AwsDesigner/ViewModels/CommandsViewModel.cs](AwsDesigner/ViewModels/CommandsViewModel.cs)

**High-level Architecture & Concepts**
- **Pattern:** MVVM (Model-View-ViewModel). Views are thin XAML + code-behind. ViewModels contain state and commands.
- **Workspace:** `WorkspaceViewModel` owns the canvas state: `Nodes` and `Connections` collections, `SelectedNode`, and exposes a `CommandsViewModel` to wire toolbar and toolbox actions.
- **Node:** `NodeViewModel` has `X`, `Y`, `Width`, `Height`, `Title`, `ResourceType`. Node views are represented by `NodeControl` which uses pointer events to implement dragging. Nodes are positioned in the `Canvas` via `Canvas.Left` and `Canvas.Top` bound to `X` and `Y`.
- **Connections:** `ConnectionViewModel` stores `Source` and `Target` node references and computes a `PathGeometry` for a bezier path between node centers. It subscribes to source/target property changes so connection visuals update live when nodes move.
- **Commands:** `CommandsViewModel` (simple `ICommand` wrappers) exposes Add/Connect/Deploy actions used by toolbar and toolbox.
- **Deploy stub:** `WorkspaceViewModel.DeployAsync()` is a placeholder; integrate AWS SDK / CloudFormation here later.

**Key Implementation Notes**
- `ItemsControl` uses `ItemsSource` bound to `Workspace.Nodes` / `Workspace.Connections` to render dynamic collections.
- The ordering of layers (connections then nodes) ensures nodes render above connection paths.
- Dragging: `NodeControl` handles pointer events, updates `NodeViewModel.X/Y`, and sets `Workspace.SelectedNode` on pointer press so the inspector can show node details.
- `ConnectionViewModel` attaches `PropertyChanged` handlers to `Source` and `Target` to call `OnPropertyChanged(nameof(PathGeometry))` when `X` or `Y` change.

**Files to Inspect First**
- [Views/MainWindow.axaml](AwsDesigner/Views/MainWindow.axaml) — primary layout: toolbox, canvas, inspector.
- [Views/NodeControl.axaml](AwsDesigner/Views/NodeControl.axaml) — node presentation.
- [ViewModels/WorkspaceViewModel.cs](AwsDesigner/ViewModels/WorkspaceViewModel.cs) — core app logic for nodes/connections and deploy stub.

**Run & Debug Tips**
- To build and run from the app folder:

```bash
cd AwsDesigner
dotnet build
dotnet run
```

- If you make XAML changes, build errors often point to invalid property names (e.g., `Padding` on `StackPanel` is invalid — use `Margin`).
- If Avalonia theme or control types cannot be resolved, ensure these packages are present in `AwsDesigner.csproj` (we include `Avalonia`, `Avalonia.ReactiveUI`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`).

**Troubleshooting — Errors Already Encountered**
- Missing `FluentTheme` error: resolved by adding `Avalonia.Themes.Fluent` package.
- `UsePlatformDetect` missing: resolved by adding `Avalonia.Desktop` package.
- `Items` binding on `ItemsControl`: use `ItemsSource` instead.
- `BezierSegment` constructor: the Avalonia API uses property setters (Point1/Point2/Point3) rather than a multi-arg ctor.
- XAML property availability: StackPanel does not have `Padding` — use `Margin`.

**Extending for AWS Integration (next steps)**
1. Add a model layer for resources (e.g., `AwsResource` base and typed subclasses `Ec2Instance`, `S3Bucket`).
2. Implement a translator that converts `WorkspaceViewModel.Nodes` + `Connections` to CloudFormation templates or calls SDK APIs.
   - Place the integration inside `WorkspaceViewModel.DeployAsync()` or a separate service `IAwsDeploymentService`.
3. Add authentication wiring (AWS credentials, profiles, MFA) and a minimal settings UI.
4. Consider introducing a serialization format (JSON/YAML) to save/load diagrams.
5. Add undo/redo support using a command history for node moves and edits.

**Testing & Validation**
- Unit-test viewmodels (add xUnit/NUnit as needed). Focus on:
  - `WorkspaceViewModel.AddNode`, `TryConnect`, `RemoveNode` behavior.
  - `ConnectionViewModel.PathGeometry` correctness for simple known coordinates.
- UI tests: Avalonia supports integration tests; start with small manual QA flows for drag/drop and connecting nodes.

**Where to Add New Code**
- Small UI controls → `Views/`
- ViewModel logic → `ViewModels/`
- If adding a service (AWS/IO), create `Services/` and register via a simple factory or DI container if you introduce one later.

**Coding Conventions & Notes**
- Prefer keeping logic in ViewModels and using simple code-behind only for interactions that cannot be expressed in XAML/commands (e.g., pointer capture for drag).
- Keep views declarative; use data templates for nodes and connections.

**Next Suggested Tasks**
- Add README (this file) — done.
- Replace `DeployAsync` stub with an `IAwsDeploymentService` abstraction and a local mock implementation.
- Add Save/Load for diagrams (JSON) and implement a small preferences/settings UI for AWS credentials selection.

If you want, I can now:
- Add a `Services/IAwsDeploymentService` mock and wiring, or
- Add Save/Load JSON for diagrams, or
- Clean up the small nullability warnings remaining in `ConnectionViewModel`.

---
Generated for the `AwsDesigner` project in this workspace. If you want a condensed printed architecture diagram or a step-by-step contributor guide, I can produce that next.
