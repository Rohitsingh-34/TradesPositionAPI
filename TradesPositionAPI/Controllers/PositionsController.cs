using Microsoft.AspNetCore.Mvc;
using TradesPositionAPI.Models;
using TradesPositionAPI.Services;

namespace TradesPositionAPI.Controllers
{

        [ApiController]
        [Route("[controller]")]
        public class PositionsController : ControllerBase
        {
            private readonly ILogger<PositionsController> _logger;
            private readonly PositionService _positionservice;
            public PositionsController(ILogger<PositionsController> logger, PositionService positionService)
            {
                _logger = logger;
                _positionservice = positionService;
            }

            [HttpGet("{user}")]
            public List<Position> GetTrades(string user)
            {
                return _positionservice.GetPositionByUser(user);
            }

        }
}
