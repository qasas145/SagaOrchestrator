using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;

namespace PaymentService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private static readonly List<Payment> Payments = new();
        [HttpPost]
        public IActionResult ProcessPayment([FromBody] JsonElement request)
        {

            int orderId = request.GetProperty("id").GetInt32();
            decimal amount = request.GetProperty("amount").GetDecimal();
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Status = "Processed"
            };
            Payments.Add(payment);
            return Ok(payment);
        }
    }
}
