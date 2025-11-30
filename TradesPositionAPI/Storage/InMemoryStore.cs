using Microsoft.OpenApi;
using TradesPositionAPI.Models;

namespace TradesPositionAPI.Storage
{
    public class InMemoryStore
    {
        public List<Models.Trade> Trades { get; set; } = new List<Models.Trade>();
        public Dictionary<string, List<Models.Position>> Positions { get; set; } = new Dictionary<string, List<Models.Position>>();


        public InMemoryStore() { }

        public void AddTrade(Trade trade)
        {

            Trades.Add(trade);
        }
        public List<Trade> GetTradesByUser(string user)
        {
            if (user == null || user == "")
            {
                return Trades;
            }

            return Trades.Where(t => t.User == user).ToList();
        }
        public void UpdatePosition(Trade trade)
        {
            List<Trade> userTrades = GetTradesByUser(trade.User);

            List<Trade> tradesbystock = userTrades.Where(t => t.Stock == trade.Stock).ToList();

            int totalQuantity = tradesbystock
                    .Where(t => t.Operation == "BUY")
                    .Sum(t => t.Quantity) -
                    tradesbystock
                    .Where(t => t.Operation == "SELL")
                    .Sum(t => t.Quantity);

            double totalBuyAmount = tradesbystock
                .Where(t => t.Operation == "BUY")
                .Sum(t => (double)t.Price * t.Quantity);

            int totalBuyQty = tradesbystock
                .Where(t => t.Operation == "BUY")
                .Sum(t => t.Quantity);

            Position position = new Position
            {
                Stock = trade.Stock,
                TotalQuantity = totalQuantity,

                AveragePrice =
                    totalBuyQty == 0 ? 0.0 : totalBuyAmount / totalBuyQty
            };

            if (Positions.ContainsKey(trade.User))
            {
                List<Position> userPositions = Positions[trade.User];
                Position? existingPosition = userPositions.FirstOrDefault(p => p.Stock == trade.Stock);
                if (existingPosition != null)
                {
                    existingPosition.TotalQuantity = position.TotalQuantity;
                    existingPosition.AveragePrice = position.AveragePrice;
                }
                else
                {
                    userPositions.Add(position);
                }
            }
            else
            {
                Positions[trade.User] = new List<Position> { position };
            }
        }

        public List<Position> GetPositionsByUser(string user)
        {
            if (Positions.ContainsKey(user))
            {
                return Positions[user];
            }
            return new List<Position>();
        }
    }
}