using Microsoft.Extensions.Logging;
using Rademaker.IotEdge.TwinProperties;

internal class DesiredPropertiesHandler : IDesiredPropertyChangedHandler, IInitialDesiredPropertyValueHandler
{
    private readonly ILogger<DesiredPropertiesHandler> logger;

    public DesiredPropertiesHandler(ILogger<DesiredPropertiesHandler> logger)
    {
        this.logger = logger;
    }
    public Task OnInitialValueAsync(string propertyName, TwinPropertyValue initialValue)
    {
        logger.LogInformation("Initial value of property {PropertyName} is {PropertyValue}", propertyName, initialValue.ReadAs<string>());
        return Task.CompletedTask;
    }

    public Task PropertyUpdatedAsync(string propertyName, TwinPropertyValue newValue)
    {
        logger.LogInformation("Updated value of property {PropertyName} is {PropertyValue}", propertyName, newValue.ReadAs<string>());
        return Task.CompletedTask;
    }
}
