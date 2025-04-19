using Avalonia.Controls;
using Avalonia.Input;
using Password_Manager.Models;
using Password_Manager.Services;
using Password_Manager.ViewModels;
using System;
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
            var authWindow = new Auth();
            authWindow.Show();
            Close();
        }

        /// <summary>
        /// Handles the click event on a password entry to edit it.
        /// </summary>
        private async void OnEntryClicked(object sender, PointerPressedEventArgs e)
        {
            try
            {
                if (sender is not Border border)
                    throw new Exception("Sender is not a Border.");

                var entry = border.DataContext as PasswordEntryModel;
                if (entry == null)
                    throw new Exception("DataContext is not a PasswordEntryModel.");

                var editWindow = new EditEntryWindow(entry);
                var result = await editWindow.ShowDialog<PasswordEntryModel>(this);
                if(string.IsNullOrEmpty(result.ServiceName) || string.IsNullOrEmpty(result.EncryptedPassword))
                {
                    return;
                }

                entry.ServiceName = result.ServiceName;
                entry.Username = result.Username;
                entry.EncryptedPassword = result.EncryptedPassword;

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
            }
        }
    }
}