using System;
using Google.Protobuf.WellKnownTypes;
using TaskApp.Protos;

namespace TaskApp.Server.Models;

public class PBTaskStore
{
    private readonly Dictionary<int, PBTask> _tasks = new();
    private int _nextTaskId = 1;
    private static readonly Random _random = new();

    public PBTaskStore()
    {
        // Initialize with 5 random tasks
        var titles = new[] { "Fix Bug", "Write Report", "Plan Meeting", "Update Docs", "Test Feature" };
        var descriptions = new[] { "Urgent fix needed", "Draft for review", "Schedule with team", "Revise user guide", "Run test cases" };
        var tags = new[] { "urgent", "planning", "docs", "testing", "bug" };

        for (int i = 0; i < 5; i++)
        {
            var task = new PBTask
            {
                Id = _nextTaskId++,
                Title = titles[_random.Next(titles.Length)],
                Description = descriptions[_random.Next(descriptions.Length)],
                Status = (PBTaskStatus)_random.Next(0, 3), // TODO, IN_PROGRESS, DONE
                Metadata = new PBTaskMetadata
                {
                    CreatedBy = "System",
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-_random.Next(1, 10))),
                    UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            };
            task.Tags.AddRange(new[] { tags[_random.Next(tags.Length)], tags[_random.Next(tags.Length)] });
            _tasks[task.Id] = task;
        }
    }

    public PBTask Add(CreatePBTaskRequest request, string createdBy)
    {
        var newTask = new PBTask
        {
            Id = _nextTaskId++,
            Title = request.Title,
            Description = request.Description,
            Status = PBTaskStatus.Todo,
            Metadata = new PBTaskMetadata
            {
                CreatedBy = createdBy,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        };

        newTask.Tags.AddRange(request.Tags);

        _tasks[newTask.Id] = newTask;

        return newTask;
    }

    public PBTask? Get(int id) => _tasks.TryGetValue(id, out var task) ? task : null;

    public bool Update(UpdatePBTaskRequest request)
    {
        if (!_tasks.TryGetValue(request.Id, out var task))
            return false;

        if (request.HasTitle) task.Title = request.Title;
        if (request.HasDescription) task.Description = request.Description;
        if (request.HasStatus) task.Status = request.Status;
        if (request.Tags.Count > 0)
        {
            task.Tags.Clear();
            task.Tags.AddRange(request.Tags);
        }

        task.Metadata.UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow);

        return true;
    }

    public bool Delete(int id)
        => _tasks.Remove(id);

    public List<PBTask> GetAll()
        => _tasks.Values.ToList();
}