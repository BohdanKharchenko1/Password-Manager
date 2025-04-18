using Avalonia.Controls;
using Avalonia.Interactivity;
using Password_Manager.Models;

namespace Password_Manager.Views
{
    public partial class EditEntryWindow : Window
    {
        private PasswordEntryModel Entry { get; }

        public EditEntryWindow(PasswordEntryModel entry)
        {
            InitializeComponent();
            // Create a copy for editing so that cancellation does not modify the original.
            Entry = new PasswordEntryModel 
            {
                ServiceName = entry.ServiceName,
                Username = entry.Username,
                EncryptedPassword = entry.EncryptedPassword
            };
            DataContext = Entry;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Return the updated entry.
            Close(Entry);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel editing.
            Close(null);
        }
    }
}