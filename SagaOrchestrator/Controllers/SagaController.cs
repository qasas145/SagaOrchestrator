using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace SagaOrchestrator.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SagaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SagaController(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;


        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] Order order)
        {
            order.Status = "Pending";
            // Step 1: Create Order
            var orderResponse = await CreateOrder(order);
            if (!orderResponse.IsSuccessStatusCode)
            {
                return BadRequest("Order creation failed.");
            }

            // Step 2: Reserve Inventory
            var inventoryResponse = await ReserveInventory(order);
            if (!inventoryResponse.IsSuccessStatusCode)
            {
                await CancelOrder(order.Id);
                return BadRequest("Inventory reservation failed.");
            }
            // Step 3: Process Payment
            var paymentResponse = await ProcessPayment(order);
            if (!paymentResponse.IsSuccessStatusCode)
            {
                await ReleaseInventory(order.ProductId, order.Quantity);
                await CancelOrder(order.Id);
                return BadRequest("Payment processing failed.");
            }
            order.Status = "Completed";
            return Ok(order);
        }
        private async Task<HttpResponseMessage> CreateOrder(Order order)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PostAsJsonAsync("http://localhost:5021/api/orders", order);
        }
        private async Task<HttpResponseMessage> ReserveInventory(Order order)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PostAsJsonAsync("http://localhost:5162/api/inventory/reserve", new { order.ProductId, order.Quantity });
        }
        private async Task<HttpResponseMessage> ProcessPayment(Order order)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PostAsJsonAsync("http://localhost:5278/api/payments", new { order.Id, Amount = order.Quantity * 10 });
        }
        private async Task<HttpResponseMessage> CancelOrder(int orderId)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.DeleteAsync($"http://localhost:5021/api/orders/{orderId}");
        }
        private async Task<HttpResponseMessage> ReleaseInventory(string productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.PostAsJsonAsync("http://localhost:5162/api/inventory/release", new { productId, quantity });
        }
    }
}
