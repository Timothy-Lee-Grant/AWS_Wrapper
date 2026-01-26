using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AwsVisualDesigner.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    public string Message {get;} = "This is my message";
    [ObservableProperty] private string _privateMessage = "This is my private message";
    public List<string> Resource {get;} = new() {"Lambda Function", "RDS Database", "EC2"};
    [ObservableProperty] private string? _selectedResource;
    [ObservableProperty] private ViewModelBase? _currentResourceForm;

    partial void OnSelectedResourceChanged(string? value)
    {
        Console.WriteLine($"I just got the data and you selected {value}");
        //Eventually I will need to create the AWS resource based on which resouce they selected.

        //determine resource selected
        if (value == "Lambda Function")
        {
            
        }
    }

    [RelayCommand]
    private void SubmitResource()
    {
        // For now, just a check
        Console.WriteLine("Submit button pressed!");

        if (CurrentResourceForm is LambdaFormViewModel lambdaVm)
        {
            // 2. Extract the data from that specific ViewModel
            string funcName = lambdaVm.Name; 
            
            // 3. Send it to your logic layer (Service)
            //_awsService.GenerateLambdaJson(funcName);
            
            Console.WriteLine($"Successfully sent {funcName} to the generator!");
        }
    }
}
