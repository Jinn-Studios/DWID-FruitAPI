using FruitUI.API.ServiceCore.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FruitUI.API
{
    public class FruitController : ControllerBase
    {
        private readonly ILogger _logger;

        public FruitController(ILogger<FruitController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Currently just returns a hard-coded list of fruits
        /// </summary>
        /// <remarks>
        /// These are Remarks<br />
        /// Line Spacing?<br />
        /// 
        ///     testCode();
        /// </remarks>
        [HttpGet("api/Fruits")] //, JinnAuthorize
        [Produces("application/json")]
        public IEnumerable<FruitDTO> GetFruits()
        {
            using var _ = _logger.BeginScope("Method Begin");
            _logger.LogInformation("Arrived in {methodName}", nameof(GetFruits));
            return new List<FruitDTO>
            {
                new FruitDTO{ FruitID = 1, Name = "Kiwi", Calories = 345 },
                new FruitDTO{ FruitID = 2, Name = "Apple", Calories = 234 },
                new FruitDTO{ FruitID = 3, Name = "Banana", Calories = 123 },
            };
        }
    }
}