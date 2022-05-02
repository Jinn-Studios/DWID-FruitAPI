using Microsoft.AspNetCore.Mvc;

namespace FruitUI.API
{
    [ApiController]
    [Route("api/Telemetry")]
    public class TelemetryController : ControllerBase
    {
        private readonly ILogger<TelemetryController> _logger;
        private readonly ICustomTelemetryChannel _telemetry;

        public TelemetryController(ILogger<TelemetryController> logger, ICustomTelemetryChannel telemetry)
        {
            _logger = logger;
            _telemetry = telemetry;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("TestFail")]
        public IEnumerable<string> TestFailure()
        {
            using var x = _logger.BeginScope("GetThingsWTF: {scopedWTFvalue}", "someValueWTF");
            _logger.LogInformation("WTF Info");
            try
            {
                throw new Exception("Did you SEE WTF just happened!?");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WTF Error");
            }
            return Enumerable.Empty<string>();
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<TelemetryModel> GetTelemetryData()
            => _telemetry.Read();
    }
}