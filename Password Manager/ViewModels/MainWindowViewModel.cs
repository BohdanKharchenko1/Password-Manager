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
        public UserModel? User { get; }
        public ObservableCollection<PasswordEntryModel> PasswordEntries { get; } = new();
        
        private PasswordEntryModel? _selectedEntry;
        public PasswordEntryModel? SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }

        public ICommand? AddEntryCommand { get; }
        public ICommand? DeleteEntryCommand { get; }

        public MainWindowViewModel(UserModel user)
        {
            try
            {
                User = user;
                _ = LoadEntriesAsync();
                AddEntryCommand = new RelayCommand(AddEntry);
                DeleteEntryCommand = new AsyncRelayCommand(DeleteSelectedEntry, () => true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error initializing MainWindowViewModel: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes the selected password entry from the encrypted file and the observable collection.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation; removes the entry from the collection if successful.</returns>
        private async Task DeleteSelectedEntry()
        {
            try
            {
                if (SelectedEntry == null)
                    return;

                bool success = await FileService.DeleteEncryptedEntryAsync(SelectedEntry.Id, User!.EntriesFilePath, User!.MasterPassword);
                if (success)
                {
                    PasswordEntries.Remove(SelectedEntry);
                }
                else
                {
                    Console.WriteLine("Failed to delete the selected entry.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting entry: {e.Message}");
            }
        }

        /// <summary>
        /// Loads encrypted password entries from the user's file and populates the observable collection.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task LoadEntriesAsync()
        {
            try
            {
                var entries = await FileService.LoadEncryptedEntriesAsync(User!.EntriesFilePath, User!.MasterPassword);
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        PasswordEntries.Add(entry);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading entries: {e.Message}");
            }
        }

        /// <summary>
        /// Adds a new password entry to the observable collection and saves it to the encrypted file.
        /// </summary>
        /// <returns>No return value; adds the entry to the collection and initiates async save.</returns>
        private void AddEntry()
        {
            try
            {
                var newEntry = new PasswordEntryModel
                {
                    ServiceName = "New Service",
                    Username = "example@domain.com",
                    EncryptedPassword = "encrypted_placeholder"
                };

                PasswordEntries.Add(newEntry);
                _ = FileService.SaveEncryptedEntryAsync(newEntry, User!.EntriesFilePath, User!.MasterPassword);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding entry: {e.Message}");
            }
        }
    }
}