using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Password_Manager.Models;
using Password_Manager.Services;

namespace Password_Manager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // The authenticated user's details are passed in (e.g., via UserModel)
        public UserModel User { get; }

        public ObservableCollection<PasswordEntryModel> PasswordEntries { get; } = new();
        
        private PasswordEntryModel? _selectedEntry;
        public PasswordEntryModel? SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }

        public ICommand AddEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }

        public MainWindowViewModel(UserModel user)
        {
            User = user;
            _ = LoadEntriesAsync();
            AddEntryCommand = new RelayCommand(AddEntry);
            DeleteEntryCommand = new RelayCommand(DeleteSelectedEntry, () => true);

        }
        private async void DeleteSelectedEntry()
        {
            if (SelectedEntry == null)
                return;

            // Call the delete method with the ID of the selected entry.
            bool success = await FileService.DeleteEncryptedEntryAsync(SelectedEntry.Id, User.EntriesFilePath, User.MasterPassword);
            if (success)
            {
                PasswordEntries.Remove(SelectedEntry);
            }
            else
            {
                Console.WriteLine("Failed to delete the selected entry.");
            }
        }


        private async Task LoadEntriesAsync()
        {
            var entries = await FileService.LoadEncryptedEntriesAsync(User.EntriesFilePath, User.MasterPassword);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    PasswordEntries.Add(entry);
                }
            }
        }

        private void AddEntry()
        {
            
            var newEntry = new PasswordEntryModel
            {
                ServiceName = "New Service",
                Username = "example@domain.com",
                EncryptedPassword = "encrypted_placeholder" // This field is normally managed via encryption.
            };

            PasswordEntries.Add(newEntry);
            _ = FileService.SaveEncryptedEntryAsync(newEntry, User.EntriesFilePath, User.MasterPassword);

        }
    }
}
