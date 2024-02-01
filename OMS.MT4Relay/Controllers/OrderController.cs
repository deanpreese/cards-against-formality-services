using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

using OMS.Core.Models;
using OMS.Services.Queue;


namespace OMS.MT4Relay.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
				
        private readonly NewOrderQueue _newOrderQueue;
        private readonly ClosedOrderQueue _closedOrderQueue;
        private ILogger<OrderController> _logger;
        
        public OrderController(ILogger<OrderController> logger, NewOrderQueue newOrderQueue, ClosedOrderQueue closedOrderQueue)
        {
            _newOrderQueue = newOrderQueue;
            _closedOrderQueue = closedOrderQueue;
            _logger = logger;
        }

        [HttpPost("process-order")]
        public async Task<string> ProcessOrder([FromBody] NewOrder newOrder)
        {
			
            int om_id = 87654321;

            if (newOrder == null)
            {
                return (BadRequest("Invalid NewOrder data").ToString());
            }

            _logger.LogInformation("New Order: " + newOrder.UserID + "  " + newOrder.Instrument + " " + newOrder.OrderPX + "  " + newOrder.OrderAction);

            try
            {
                await _newOrderQueue.WriteAsync(newOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                om_id = 0;
            }

            return "Order processed successfully " + om_id;
        }
    }
}
