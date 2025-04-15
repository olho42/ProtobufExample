using System.Windows;
using TaskApp.Client.ViewModels;
using TaskApp.Protos;

namespace TaskApp.Client.Views;

public partial class CreatePBTaskWindow : Window
{
    public CreatePBTaskWindow(PBTask? task = null)
    {
        InitializeComponent();
        DataContext = new CreatePBTaskViewModel(task);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
