using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rademaker.IotEdge.Inputs;

public class MyInputHandler : IInputHandler
{
    private readonly ILogger<MyInputHandler> logger;

    public MyInputHandler(ILogger<MyInputHandler> logger)
    {
        this.logger = logger;
    }
    public Task OnInputAsync(InputMessage message)
    {
        logger.LogInformation("Input received:  {Input}, Body: {Body}", message.InputName, JsonSerializer.Serialize(message.ReadAs<JsonDocument>(), new JsonSerializerOptions { WriteIndented = true }));
        return Task.CompletedTask;
    }
}
