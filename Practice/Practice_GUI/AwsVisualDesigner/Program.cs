using Avalonia;
using System;

namespace AwsVisualDesigner;

class Program
{
    // This is the "Main" method the compiler is looking for!
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}