using Microsoft.Extensions.Logging;
using Rademaker.IotEdge.Methods;

public class TimesTwoMethodHandler : AbstractMethodHandler<int>
{
    public TimesTwoMethodHandler(ILogger<TimesTwoMethodHandler> logger) : base(logger)
    {
    }

    public override Task<MethodInvocationResponse> HandleMethodRequestAsync(int body)
    {
        return Task.FromResult(MethodInvocationResponse.Success(body * 2));
    }
}
