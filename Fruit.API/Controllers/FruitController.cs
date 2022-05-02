using Microsoft.AspNetCore.Mvc;

namespace Fruit.API.Controllers
{
    public class FruitController : ControllerBase
    {
        [HttpGet("api/Fruits")]
        public IEnumerable<FruitDTO> GetFruits()
        {
            return new List<FruitDTO>
            {
                new FruitDTO{ FruitID = 1, Name = "Kiwi", Calories = 345 },
                new FruitDTO{ FruitID = 2, Name = "Apple", Calories = 234 },
                new FruitDTO{ FruitID = 3, Name = "Banana", Calories = 123 }
            };
        }
    }
}