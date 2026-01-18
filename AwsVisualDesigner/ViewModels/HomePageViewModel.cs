using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace AwsVisualDesigner.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    public string Message {get;} = "This is my message";
    [ObservableProperty] private string _privateMessage = "This is my private message";
    public List<string> Regions {get;} = new() {"east-1", "east-2", "west-1"};
    [ObservableProperty] private string? _selectedRegion;

    partial void OnSelectedRegionChanged(string? item)
    {
        Console.WriteLine($"I just got the data and you selected {item}");
    }
}