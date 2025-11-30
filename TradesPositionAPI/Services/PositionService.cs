using TradesPositionAPI.Models;
using TradesPositionAPI.Storage;

namespace TradesPositionAPI.Services
{
    public class PositionService
    {
        readonly InMemoryStore _store;
        readonly TradeService _tradeService;
        public PositionService(InMemoryStore store, TradeService tradeService)
        {
            _store = store;
            _tradeService = tradeService;
        }

        public List<Position> GetPositionByUser(string user)
        {
            return _store.GetPositionsByUser(user.ToLower());
        }

    }
}
