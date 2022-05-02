using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace FruitUI.API
{
    public class CustomTelemetryChannel : ICustomTelemetryChannel
    {
        private readonly object _flushLock = new();
        private List<TelemetryModel> _flushed = new();

        public bool? DeveloperMode { get; set; }
        public string? EndpointAddress { get; set; }

        public List<RequestTelemetry> Requests { get; set; } = new();
        public List<TraceTelemetry> Traces { get; set; } = new();
        public List<ExceptionTelemetry> Exceptions { get; set; } = new();
        public List<MetricTelemetry> Metrics { get; set; } = new();
        public List<ITelemetry> OtherTelemetry { get; set; } = new();

        public List<TelemetryModel> Read()
        {
            Flush();
            List<TelemetryModel> flushed;
            var timeout = DateTime.UtcNow.AddMinutes(-5);
            lock (_flushLock)
            {
                flushed = _flushed.Select(x => x with { }).ToList();
                _flushed.RemoveAll(x => x.Timestamp < timeout);
            }
            return flushed;
        }

        public void Flush()
        {
            var timeout = DateTime.UtcNow.AddMinutes(-5);
            lock (_flushLock)
            {
                _flushed.AddRange(Exceptions.Select(x => BaseTelemetry(x, () => x.Properties, () => x.Exception.Message)));
                Exceptions.Clear();
                _flushed.AddRange(Traces.Select(x => BaseTelemetry(x, () => x.Properties, () => x.Message)));
                Traces.Clear();
                _flushed.AddRange(Requests.Select(x => BaseTelemetry(x, () => x.Properties, () => x.Url.ToString())));
                Requests.Clear();

                _flushed = _flushed.OrderBy(x => x.Timestamp).ToList();
                if (_flushed.Count > 10000)
                    _flushed.RemoveAll(x => x.Timestamp < timeout);
            }
        }

        public void Send(ITelemetry item)
        {
            if (typeof(RequestTelemetry).IsAssignableFrom(item.GetType()))
            {
                if (((RequestTelemetry)item).Url.ToString().Contains("Telemetry"))
                    return;
            }
            lock (_flushLock)
            {
                if (typeof(RequestTelemetry).IsAssignableFrom(item.GetType()))
                    Requests.Add((RequestTelemetry)item);
                else if (typeof(TraceTelemetry).IsAssignableFrom(item.GetType()))
                    Traces.Add((TraceTelemetry)item);
                else if (typeof(ExceptionTelemetry).IsAssignableFrom(item.GetType()))
                    Exceptions.Add((ExceptionTelemetry)item);
                else if (typeof(MetricTelemetry).IsAssignableFrom(item.GetType()))
                    Metrics.Add((MetricTelemetry)item);
            }
        }

        private static TelemetryModel BaseTelemetry(ITelemetry x, Func<IDictionary<string, string>> getProps, Func<string> getInfo)
        {
            return new TelemetryModel
            {
                TelemetryType = x.GetType().Name,
                Timestamp = x.Timestamp,
                Operation = x.Context.Operation,
                GlobalProperties = x.Context.GlobalProperties,
                Properties = getProps(),
                Information = getInfo()
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}