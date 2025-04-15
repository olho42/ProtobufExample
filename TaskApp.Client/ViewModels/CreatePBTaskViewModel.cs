using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using System.Linq;
using TaskApp.Client.Services;
using TaskApp.Protos;

namespace TaskApp.Client.ViewModels;

public partial class CreatePBTaskViewModel : ObservableObject
{
    private readonly PBTaskServiceClient _client;
    private readonly PBTask? _existingTask;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private PBTaskStatus _status;

    [ObservableProperty]
    private string _tags = string.Empty;

    public CreatePBTaskViewModel(PBTask? pbTask = null)
    {
        _client = new PBTaskServiceClient();
        _existingTask = pbTask;

        if (pbTask != null)
        {
            Title = pbTask.Title;
            Description = pbTask.Description;
            Status = pbTask.Status;
            Tags = string.Join(",", pbTask.Tags);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return;

        if (_existingTask == null)
        {
            var request = new CreatePBTaskRequest
            {
                Title = Title,
                Description = Description,
                Tags = { Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()) }
            };

            await _client.CreatePBTaskAsync(request);
        }
        else
        {
            var request = new UpdatePBTaskRequest { Id = _existingTask.Id };

            if (Title != _existingTask.Title) request.Title = Title;

            if (Description != _existingTask.Description) request.Description = Description;

            if (Status != _existingTask.Status)
                request.Status = Status;

            var newTags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());
            if (!newTags.SequenceEqual(_existingTask.Tags)) request.Tags.AddRange(newTags);

            if (request.Title != null || request.Description != null || request.Status != null || request.Tags.Count > 0)
                await _client.UpdatePBTaskAsync(request);
        }
    }
}