
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;

namespace AnatomyIotEdge;

internal class Program
{
    private static void Main()
    {
        Init().Wait();

        // Wait until the app unloads or is cancelled
        CancellationTokenSource cts = new();
        AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
        Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
        WhenCancelled(cts.Token).Wait();
    }

    /// <summary>
    /// Handles cleanup operations when app is cancelled or unloads
    /// </summary>
    public static Task WhenCancelled(CancellationToken cancellationToken)
    {
        TaskCompletionSource<bool> tcs = new();
        cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
        return tcs.Task;
    }

    /// <summary>
    /// Initializes the ModuleClient and sets up the callback to receive
    /// messages containing temperature information
    /// </summary>
    private static async Task Init()
    {
        MqttTransportSettings mqttSetting = new(TransportType.Mqtt_Tcp_Only);
        ITransportSettings[] settings = { mqttSetting };

        // Open a connection to the Edge runtime
        ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
        await ioTHubModuleClient.OpenAsync();
        Console.WriteLine("IoT Hub module client initialized.");

        MethodInvocations();
        InputAndOutputs();
        DesiredAndReportedProperties();


        void MethodInvocations()
        {
            ioTHubModuleClient.SetMethodHandlerAsync("MyMethod", MethodCallback, null);

            Task<MethodResponse> MethodCallback(MethodRequest methodRequest, object userContext)
            {
                // MethodResponse response = new MethodResponse(methodRequest.Data, 200);
                byte[] responseAsBytes = JsonSerializer.SerializeToUtf8Bytes(new { Hello = "World" });
                MethodResponse response = new MethodResponse(responseAsBytes, 200);
                return Task.FromResult(response);
            }
        }

        void InputAndOutputs()
        {
            ioTHubModuleClient.SetInputMessageHandlerAsync("input1", InputMessageHandler, null);

            async Task<MessageResponse> InputMessageHandler(Message message, object userContext)
            {
                byte[] responseAsBytes = JsonSerializer.SerializeToUtf8Bytes(new { Hello = "World" });
                await ioTHubModuleClient.SendEventAsync("output1", new Message(responseAsBytes));
                return MessageResponse.Completed;
            }
        }

        void DesiredAndReportedProperties()
        {
            ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyCallback, null);

            async Task DesiredPropertyCallback(TwinCollection desiredProperties, object userContext)
            {
                TwinCollection twinCollection = new();
                foreach (KeyValuePair<string, object> property in desiredProperties)
                {
                    JToken token = (JToken)property.Value;
                    twinCollection[property.Key + "-processed"] = token;
                    twinCollection[property.Key] = null;
                }

                await ioTHubModuleClient.UpdateReportedPropertiesAsync(twinCollection);
            }
        }
    }
}
