using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [AllowAnonymous]
        [HttpGet("webhooks/payos")]
        [HttpPost("webhooks/payos")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                var method = Request.Method;
                Console.WriteLine($"=== WEBHOOK RECEIVED ===");
                Console.WriteLine($"Method: {method}");
                Console.WriteLine($"Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
                Console.WriteLine($"QueryString: {Request.QueryString}");

                if (Request.Headers.ContainsKey("ngrok-skip-browser-warning"))
                {
                    Response.Headers["ngrok-skip-browser-warning"] = "true";
                }

                if (method == "POST")
                {
                    using var reader = new StreamReader(Request.Body);
                    var body = await reader.ReadToEndAsync();

                    Console.WriteLine($"Body: {body}");

                    if (!string.IsNullOrEmpty(body))
                    {
                        try
                        {
                            var payload = System.Text.Json.JsonSerializer.Deserialize<PayosWebhookPayload>(body);
                            Console.WriteLine($"Deserialized payload: OrderCode={payload?.orderCode}, Status={payload?.status}");

                            if (payload != null)
                            {
                                var ok = await _paymentService.HandlePayOSWebhook(payload);
                                Console.WriteLine($"Webhook processing result: {ok}");
                                if (!ok) return BadRequest(new { success = false, message = "Failed to process webhook" });
                            }
                            else
                            {
                                Console.WriteLine("Payload is null");
                                return BadRequest(new { success = false, message = "Invalid payload format" });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Webhook processing error: {ex.Message}");
                            return BadRequest(new { success = false, message = $"Error: {ex.Message}" });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Empty body received");
                        return BadRequest(new { success = false, message = "Empty body" });
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = $"Webhook processed successfully - {method}",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook {Request.Method} error: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
