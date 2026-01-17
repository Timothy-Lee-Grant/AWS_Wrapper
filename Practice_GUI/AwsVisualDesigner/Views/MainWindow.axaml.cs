using Avalonia.Controls;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

namespace AwsVisualDesigner.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent(); // This should stop throwing an error now
    }

    private void AddEc2_Click(object sender, RoutedEventArgs e)
    {
        var rect = new Rectangle { Width = 50, Height = 50, Fill = Brushes.Orange };
        Canvas.SetLeft(rect, 50);
        Canvas.SetTop(rect, 50);
        MainCanvas.Children.Add(rect);
    }

    private void AddS3_Click(object sender, RoutedEventArgs e)
    {
        var circle = new Ellipse { Width = 50, Height = 50, Fill = Brushes.DodgerBlue };
        Canvas.SetLeft(circle, 150);
        Canvas.SetTop(circle, 50);
        MainCanvas.Children.Add(circle);
    }
}