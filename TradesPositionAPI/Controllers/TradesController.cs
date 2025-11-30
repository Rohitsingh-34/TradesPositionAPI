using Microsoft.AspNetCore.Mvc;
using TradesPositionAPI.Models;
using TradesPositionAPI.Services;

namespace TradesPositionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly ILogger<TradesController> _logger;
        private readonly TradeService _tradeService;
        public TradesController(ILogger<TradesController> logger, TradeService tradeService)
        {
            _logger = logger;
            _tradeService = tradeService;
        }

        [HttpGet("{user}")]
        public List<Trade> Get(string user)
        {
            return _tradeService.GetTradesByUser(user);
        }

        [HttpPost("")]
        public IActionResult Post([FromBody] Trade trade)
        {
            if (trade == null) {
                return BadRequest("Trade data is required.");
            }

            if (trade.Operation.ToUpper() == "BUY" || trade.Operation.ToUpper() == "SELL") { 
            _tradeService.AddTrade(trade);
            }
            else
            {
                return BadRequest("Operation can be either BUY or SELL ");
            }

                _logger.LogInformation("Received forecast: {@Forecast}", trade);
            return Ok("Trade Added to Database Successfully!");
        }
    }
}
