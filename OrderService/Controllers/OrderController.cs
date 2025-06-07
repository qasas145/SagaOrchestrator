using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(ILogger<OrdersController> logger) => _logger = logger;

        private static readonly List<Order> Orders = new();

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            _logger.LogTrace("Starting to create the order");

            order.Id = Orders.Count + 1;
            order.Status = "Created";
            Orders.Add(order);

            _logger.LogInformation("Order Created");

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        [HttpDelete("{id}")]
        public IActionResult CancelOrder(int id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            Orders.Remove(order);
            return NoContent();
        }
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            _logger.LogTrace("In the GetOrderById Method");

            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            _logger.LogTrace("Order fetched ");
            return Ok(order);
        }
    }
}
