
using CommunityToolkit.Mvvm.ComponentModel;

namespace AwsVisualDesigner.ViewModels;

public partial class LambdaFormViewModel : ViewModelBase
{
    [ObservableProperty] private string _name {get; set;} = "Name Not Set";
}