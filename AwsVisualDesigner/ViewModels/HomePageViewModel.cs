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
}