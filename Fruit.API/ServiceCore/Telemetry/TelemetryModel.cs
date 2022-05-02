using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace FruitUI.API
{
    public record TelemetryModel
    {
        public string? TelemetryType { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public OperationContext? Operation { get; set; }
        public string? Information { get; set; }
        public IDictionary<string, string>? Properties { get; set; }
        public IDictionary<string, string>? GlobalProperties { get; set; }
    }
}