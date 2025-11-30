namespace TradesPositionAPI.Models
{
    public class Position
    {
        public string Stock { get; set; }
        public int TotalQuantity { get; set; }
        public double AveragePrice { get; set; }
    }
}
