using TradesPositionAPI.Models;
using TradesPositionAPI.Storage;

namespace TradesPositionAPI.Services
{
    public class TradeService
    {
        private readonly InMemoryStore _store;
        public TradeService(InMemoryStore store)
        {
            _store = store;
        }
        public List<Trade> GetTradesByUser(string user)
        {
            return _store.GetTradesByUser(user.ToLower());
        }
        public void AddTrade(Trade trade)
        {
            trade.User = trade.User.ToLower();
            trade.Operation = trade.Operation.ToUpper();
            trade.Stock = trade.Stock.ToUpper();
            _store.AddTrade(trade);
            _store.UpdatePosition(trade);
        }
    }
}
