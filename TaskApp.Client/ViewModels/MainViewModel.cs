using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TaskApp.Client.Services;
using TaskApp.Client.Views;
using TaskApp.Protos;

namespace TaskApp.Client.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PBTaskServiceClient _client;

    [ObservableProperty]
    private ObservableCollection<PBTask> _tasks = new();

    [ObservableProperty]
    private PBTask? _selectedTask;

    public MainViewModel()
    {
        _client = new PBTaskServiceClient();
        LoadTasksCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTasks()
    {
        var response = await _client.ListPBTasksAsync();
        Tasks.Clear();
        foreach (var task in response.Tasks)
            Tasks.Add(task);
    }

    [RelayCommand]
    private void CreateTask()
    {
        var dialog = new CreatePBTaskWindow();

        if (dialog.ShowDialog() == true)
            LoadTasksCommand.Execute(null);
    }

    [RelayCommand]
    private void UpdateTask()
    {
        if (SelectedTask == null)
        {
            MessageBox.Show("Select a task to update.");
            return;
        }

        var dialog = new CreatePBTaskWindow(SelectedTask);

        if (dialog.ShowDialog() == true)
            LoadTasksCommand.Execute(null);
    }

    [RelayCommand]
    private async Task DeleteTask()
    {
        if (SelectedTask == null)
        {
            MessageBox.Show("Select a task to delete.");
            return;
        }

        await _client.DeletePBTaskAsync(SelectedTask.Id);
        await LoadTasksCommand.ExecuteAsync(null);
    }
}