using System.Collections.ObjectModel;
using Avalonia.Controls;
using Password_Manager.ViewModels;

namespace Password_Manager.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

    }
}