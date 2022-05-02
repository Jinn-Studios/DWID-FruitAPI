namespace FruitUI.API
{
    /// <summary>
    /// Represents a Fruit and its Calories
    /// </summary>
    public record FruitDTO
    {
        /// <summary>
        /// The DB Identifier of the Fruit
        /// </summary>
        public int FruitID { get; set; }
        public string? Name { get; set; }
        public int Calories { get; set; }
    }
}