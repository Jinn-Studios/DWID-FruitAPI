using Microsoft.ApplicationInsights.Channel;

namespace FruitUI.API
{
    public interface ICustomTelemetryChannel : ITelemetryChannel
    {
        List<TelemetryModel> Read();
    }
}