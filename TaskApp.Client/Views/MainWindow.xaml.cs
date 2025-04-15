using System.Windows;
using TaskApp.Client.ViewModels;

namespace TaskApp.Client.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}