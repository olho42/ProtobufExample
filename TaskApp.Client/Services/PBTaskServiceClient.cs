using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using TaskApp.Protos;

namespace TaskApp.Client.Services;

public class PBTaskServiceClient
{
    private readonly PBTaskService.PBTaskServiceClient _client;

    public PBTaskServiceClient()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:5001");
        _client = new PBTaskService.PBTaskServiceClient(channel);
    }

    public async Task<PBTaskResponse> CreatePBTaskAsync(CreatePBTaskRequest request)
        => await _client.CreatePBTaskAsync(request);

    public async Task<PBTaskResponse> GetPBTaskAsync(int id)
        => await _client.GetPBTaskAsync(new PBTaskRequest { Id = id });

    public async Task<PBTaskResponse> UpdatePBTaskAsync(UpdatePBTaskRequest request)
        => await _client.UpdatePBTaskAsync(request);

    public async Task<DeletePBTaskResponse> DeletePBTaskAsync(int id)
        => await _client.DeletePBTaskAsync(new PBTaskRequest { Id = id });

    public async Task<PBTaskListResponse> ListPBTasksAsync()
        => await _client.ListPBTasksAsync(new Empty());
}