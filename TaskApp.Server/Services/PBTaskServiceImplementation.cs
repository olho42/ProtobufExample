using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using TaskApp.Protos;
using TaskApp.Server.Models;

namespace TaskApp.Server.Services;

public class PBTaskServiceImplementation : PBTaskService.PBTaskServiceBase
{
    private readonly PBTaskStore _store;
    private readonly ILogger<PBTaskServiceImplementation> _logger;

    public PBTaskServiceImplementation(PBTaskStore store, ILogger<PBTaskServiceImplementation> logger)
    {
        _store = store;
        _logger = logger;
    }

    public override async Task<PBTaskResponse> CreatePBTask(CreatePBTaskRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating task with title: {Title}", request.Title);

        var task = _store.Add(request, "User"); // Hardcoded for simplicity

        _logger.LogInformation("Created task with ID: {Id}", task.Id);

        return await Task.FromResult(new PBTaskResponse { Task = task });
    }

    public override async Task<PBTaskResponse> GetPBTask(PBTaskRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Retrieving task with ID: {Id}", request.Id);
        var task = _store.Get(request.Id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID: {Id} not found", request.Id);
            throw new RpcException(new Status(StatusCode.NotFound, "Task not found"));
        }

        _logger.LogInformation("Retrieved task with ID: {Id}, Title: {Title}", task.Id, task.Title);

        return await Task.FromResult(new PBTaskResponse { Task = task });
    }

    public override async Task<PBTaskResponse> UpdatePBTask(UpdatePBTaskRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating task with ID: {Id}", request.Id);

        if (!_store.Update(request))
        {
            _logger.LogWarning("Task with ID: {Id} not found for update", request.Id);
            throw new RpcException(new Status(StatusCode.NotFound, "Task not found"));
        }

        var task = _store.Get(request.Id)!;

        _logger.LogInformation("Updated task with ID: {Id}, New Title: {Title}", task.Id, task.Title);

        return await Task.FromResult(new PBTaskResponse { Task = task });
    }

    public override async Task<DeletePBTaskResponse> DeletePBTask(PBTaskRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting task with ID: {Id}", request.Id);

        var success = _store.Delete(request.Id);

        if (success)
            _logger.LogInformation("Deleted task with ID: {Id}", request.Id);
        else
            _logger.LogWarning("Task with ID: {Id} not found for deletion", request.Id);

        return await Task.FromResult(new DeletePBTaskResponse { Success = success });
    }

    public override async Task<PBTaskListResponse> ListPBTasks(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Listing all tasks");

        var tasks = _store.GetAll();
        var response = new PBTaskListResponse();
        response.Tasks.AddRange(tasks);

        _logger.LogInformation("Returned {Count} tasks", tasks.Count);

        return await Task.FromResult(response);
    }
}