using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Password_Manager.Models;
using Password_Manager.Services;
using Password_Manager.ViewModels;

namespace Password_Manager.Views
{
    public partial class Auth : Window
    {
        // Assumes you have TextBox controls named "UsernameBox" and "PasswordBox",
        // Button controls "RegisterButton" and "LoginButton", and a TextBlock "ErrorTextBlock" in your XAML.
        public Auth()
        {
            InitializeComponent();
            RegisterButton.Click += async (_, _) => await RegisterAsync();
            LoginButton.Click += async (_, _) => await LoginAsync();
        }

        private async Task RegisterAsync()
        {
            var username = UsernameBox.Text?.Trim();
            var password = PasswordBox.Text;

            // Try to register the user. Returns a UserModel if successful.
            UserModel? user = await UserService.RegisterUserAsync(username, password);
            if (user != null)
            {
                ErrorTextBlock.Text = "Registered successfully!";
                await LoginAsync();
                // Optionally, you could auto-login the user here.
            }
            else
            {
                ErrorTextBlock.Text = "User already exists or registration failed.";
            }
        }

        private async Task LoginAsync()
        {
            var username = UsernameBox.Text?.Trim();
            var password = PasswordBox.Text;

            // Attempt to verify the user's credentials.
            UserModel? user = await UserService.VerifyUserAsync(username, password);
            if (user != null)
            {
                // Successful login: open MainWindow with the authenticated user.
                var mainWindow = new MainWindow(user)
                {
                    DataContext = new MainWindowViewModel(user)
                };
                mainWindow.Show();

                // Close the Auth window.
                Close();
            }
            else
            {
                ErrorTextBlock.Text = "Invalid username or password.";
            }
        }
    }
}
