using Microsoft.Extensions.Hosting;
using Rademaker.IotEdge.Messages;

namespace AspNetIotEdgeDevice;

internal class SendMessagesHostedService : BackgroundService
{
    private readonly IIotEdgeClient iotEdgeClient;

    public SendMessagesHostedService(IIotEdgeClient iotEdgeClient)
    {
        this.iotEdgeClient = iotEdgeClient;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
            await iotEdgeClient.SendMessageAsync("output1", new { Hello = "World" });
        }
    }
}
