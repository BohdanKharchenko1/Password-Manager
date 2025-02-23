using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Password_Manager.Models;
using Password_Manager.Services;

namespace Password_Manager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<PasswordEntryModel> PasswordEntries { get; } = new();

    public MainWindowViewModel()
    {
        _ = LoadEntries();
    }

    private async Task LoadEntries()
    {
        var entries = await FileService.LoadEntriesAsync("C:\\Users\\bodi4\\RiderProjects\\Password_Manager\\Password Manager\\Entries.json");
        if (entries is not null)
        {
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
                PasswordEntries.Add(entry);
            }
        }
    }
    
    
    
}