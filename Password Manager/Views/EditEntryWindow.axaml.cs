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
            Entry = new PasswordEntryModel 
            {
                ServiceName = entry.ServiceName,
                Username = entry.Username,
                EncryptedPassword = entry.EncryptedPassword
            };
            DataContext = Entry;
        }

        /// <summary>
        /// Saves the edited password entry and closes the window.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <returns>No return value; closes the window with the updated entry.</returns>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Close(Entry);
        }

        /// <summary>
        /// Cancels editing and closes the window without saving changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <returns>No return value; closes the window with null.</returns>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}