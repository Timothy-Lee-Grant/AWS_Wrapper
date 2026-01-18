using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAvalonia.Models;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace PracticeAvalonia.ViewModels;

//This appears to be him adding things which will work with the new contact event on the right han side of the gui
public partial class ContactManagerViewModel : ViewModelBase
{
    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newEmail = string.Empty;
    [ObservableProperty] private string _newPhone = string.Empty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private Contact? _selectedContact;

    [ObservableProperty] private ObservableCollection<Contact> _contacts = new();
    public IEnumerable<Contact> FilteredContacts =>
        string.IsNullOrWhiteSpace(SearchText)
        ? Contacts
        : Contacts.Where(c => c.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));


    public ContactManagerViewModel()
    {
        SampleData();
    }

    private void SampleData()
    {
        Contacts.Add(new Contact { Name = "Tim Grant", Email = "myemail@gmail.com", Phone = "555-555-5555"});
        Contacts.Add(new Contact { Name = "james cap", Email = "jamescap@gmail.com", Phone = "555-555-5555"});
        Contacts.Add(new Contact { Name = "Tommy bottle", Email = "tommybottle@gmail.com", Phone = "555-555-5555"});
        Contacts.Add(new Contact { Name = "Speker Joe", Email = "speakerjoe@gmail.com", Phone = "555-555-5555"});
        Contacts.Add(new Contact { Name = "monitor Jason", Email = "monitorjason@gmail.com", Phone = "555-555-5555"});
    }
    [RelayCommand]
    private void AddContact()
    {
        var newContact = new Contact
        {
            Name = NewName,
            Phone = NewPhone,
            Email = NewEmail
        };
        Contacts.Add(newContact);
        NewName = NewEmail = NewPhone = string.Empty;
    }

    [RelayCommand]
    private void DeleteContact(Contact contact)
    {
        Contacts.Remove(contact);
        if(SelectedContact == contact) SelectedContact = null;
    }
}