using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Password_Manager.Services;

namespace Password_Manager.Views
{
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
            RegisterButton.Click += async (_, _) => await HandleRegisterAsync();
            LoginButton.Click += async (_, _) => await HandleLoginAsync();
        }

        private async Task HandleRegisterAsync()
        {
            try
            {
                var username = UsernameBox.Text?.Trim();
                var password = PasswordBox.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                {
                    ErrorTextBlock.Text = "Enter both username and password.";
                    return;
                }

                if (!IsPasswordComplex(password))
                {
                    ErrorTextBlock.Text = "Password must be at least 8 characters and include uppercase, lowercase, digit, and special character.";
                    return;
                }

                var user = await UserService.RegisterUserAsync(username, password);
                if (user == null)
                {
                    ErrorTextBlock.Text = "Registration failed or user already exists.";
                    return;
                }

                ErrorTextBlock.Text = string.Empty;
                await HandleLoginAsync();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Error during registration: " + ex.Message;
            }
        }

        private async Task HandleLoginAsync()
        {
            try
            {
                var username = UsernameBox.Text?.Trim();
                var password = PasswordBox.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                {
                    ErrorTextBlock.Text = "Enter both username and password.";
                    return;
                }

                var user = await UserService.VerifyUserAsync(username, password);
                if (user == null)
                {
                    ErrorTextBlock.Text = "Invalid username or password.";
                    return;
                }

                ErrorTextBlock.Text = string.Empty;
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Error during login: " + ex.Message;
            }
        }

        private bool IsPasswordComplex(string password)
        {
            // Check for at least one uppercase, one lowercase, one digit, and one special character
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }
    }
}