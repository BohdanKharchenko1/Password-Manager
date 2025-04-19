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

        /// <summary>
        /// Handles user registration by validating input and registering the user.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation; displays error messages or proceeds to log in on success.</returns>
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
            catch (Exception e)
            {
                ErrorTextBlock.Text = "Error during registration: " + e.Message;
            }
        }

        /// <summary>
        /// Handles user login by validating credentials and opening the main window.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation; opens main window on success or displays error message.</returns>
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
            catch (Exception e)
            {
                ErrorTextBlock.Text = "Error during login: " + e.Message;
            }
        }

        /// <summary>
        /// Validates if a password meets complexity requirements.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password is at least 8 characters and contains uppercase, lowercase, digit, and special character; otherwise, false.</returns>
        private bool IsPasswordComplex(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }
    }
}