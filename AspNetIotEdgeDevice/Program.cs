using AspNetIotEdgeDevice;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rademaker.IotEdge;

await new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddLogging(logginBuilder => logginBuilder.SetMinimumLevel(LogLevel.Debug).AddJsonConsole());
        services.AddHostedService<SendMessagesHostedService>();
        services.AddIotEdge(iotEdgeBuilder =>
        {
            iotEdgeBuilder.AddInputHandlers(inputHandlers =>
            {
                inputHandlers.Handle<MyInputHandler>("input1");
            });

            iotEdgeBuilder.AddMethodHandlers(methodHandlers =>
            {
                methodHandlers.Handle<TimesTwoMethodHandler>("MyMethod");
            });
            iotEdgeBuilder.AddDesiredPropertiesHandlers(desiredProperties =>
            {
                desiredProperties.Handle<DesiredPropertiesHandler>("PropertyOne");
            });
        });
    })
    .RunConsoleAsync();
