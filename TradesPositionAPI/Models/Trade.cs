namespace TradesPositionAPI.Models
{
    public class Trade
    {
        public Guid TradeId { get; set; } = Guid.NewGuid();
        public string Stock { get; set; }
        public string Operation { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
