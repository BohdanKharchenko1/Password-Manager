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

        public ICommand AddEntryCommand { get; }

        public MainWindowViewModel(UserModel user)
        {
            User = user;
            _ = LoadEntriesAsync();
            AddEntryCommand = new RelayCommand(AddEntry);
        }


        private async Task LoadEntriesAsync()
        {
            // Load the encrypted entries via FileService using the user's file path and master password.
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
            // For demonstration purposes, we create a dummy entry.
            // In a real app, you'd show a new window or dialog to let the user input details.
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
