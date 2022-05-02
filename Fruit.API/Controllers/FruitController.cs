using Microsoft.AspNetCore.Mvc;

namespace Fruit.API.Controllers
{
    public class FruitController : ControllerBase
    {
        private readonly ILogger _logger;

        public FruitController(ILogger<FruitController> logger)
        {
            _logger = logger;
        }

        [HttpGet("api/Fruits")]
        public IEnumerable<FruitDTO> GetFruits()
        {
            _logger.LogInformation("We Made It!");
            return new List<FruitDTO>
            {
                new FruitDTO{ FruitID = 1, Name = "Kiwi", Calories = 345 },
                new FruitDTO{ FruitID = 2, Name = "Apple", Calories = 234 },
                new FruitDTO{ FruitID = 3, Name = "Banana", Calories = 123 }
            };
        }
    }
}