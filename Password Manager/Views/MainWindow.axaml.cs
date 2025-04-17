using Avalonia.Controls;
using Avalonia.Input;
using Password_Manager.Models;
using Password_Manager.Services;
using Password_Manager.ViewModels;
using System;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace Password_Manager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(UserModel user)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(user);
            LogoutButton.Click += LogOutClicked;
        }

        private void LogOutClicked(object sender, RoutedEventArgs e)
        {
            // Optionally, you can prompt the user for confirmation before logging out.
            var authWindow = new Auth();
            authWindow.Show();
            Close();
        }

        private async void OnEntryClicked(object sender, PointerPressedEventArgs e)
        {
            try
            {
                if (sender is not Border border)
                    throw new Exception("Sender is not a Border.");

                // Retrieve the item's DataContext.
                var entry = border.DataContext as PasswordEntryModel;
                if (entry == null)
                    throw new Exception("DataContext is not a PasswordEntryModel.");

                // Open the edit dialog window, passing the current entry.
                var editWindow = new EditEntryWindow(entry);
                var result = await editWindow.ShowDialog<PasswordEntryModel>(this);
                if (result == null)
                    return; // User canceled editing.

                // Update the entry with new values.
                entry.ServiceName = result.ServiceName;
                entry.Username = result.Username;
                entry.EncryptedPassword = result.EncryptedPassword;

// Force the UI to refresh the updated entry
                if (DataContext is MainWindowViewModel vm)
                {
                    var index = vm.PasswordEntries.IndexOf(entry);
                    if (index >= 0)
                    {
                        vm.PasswordEntries[index] = vm.PasswordEntries[index];
                    }

                    // Save to file
                    await FileService.SaveEncryptedEntryAsync(entry, vm.User.EntriesFilePath, vm.User.MasterPassword);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnEntryClicked Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                // Optionally display an error to the user.
            }
        }
    }
}