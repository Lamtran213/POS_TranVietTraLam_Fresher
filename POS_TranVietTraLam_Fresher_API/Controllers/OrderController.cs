using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private IAuthenticatedUser _authenticatedUser;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, IAuthenticatedUser authenticatedUser)
        {
            _logger = logger;
            _orderService = orderService;
            _authenticatedUser = authenticatedUser;
        }

        [Authorize(Roles = "User")]
        [HttpGet("orders-by-user")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO?>>> GetByUserId()
        {
            var id = _authenticatedUser.UserId;
            try
            {
                var orders = await _orderService.GetByUserId(id);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for user {UserId}", id);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDTO>> GetById(int orderId)
        {
            try
            {
                var order = await _orderService.GetById(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}", orderId);
                return NotFound(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateOrderResponseDTO>> CreateOrderFromCart(
                    [FromBody] CreateOrderRequestDTO orderRequest)
        {
            try
            {
                var result = await _orderService.CreateOrderFromCart(orderRequest);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order: {@Request}", orderRequest);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{orderId:int}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, [FromQuery] OrderStatus status)
        {
            try
            {
                var success = await _orderService.UpdateStatusOrder(orderId, status);
                if (!success)
                    return BadRequest("Update failed.");

                return Ok("Order status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", orderId);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("email/{memberId:int}")]
        public async Task<ActionResult<string>> GetCustomerEmailByMemberId(Guid memberId)
        {
            try
            {
                var email = await _orderService.GetCustomerEmailByMemberId(memberId);
                return Ok(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting email for member {MemberId}", memberId);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetAllOrders(
            [FromQuery] DateTime? orderDate,
            [FromQuery] OrderStatus? status,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await _orderService.GetAllOrders(orderDate ?? DateTime.MinValue, status, pageIndex, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountOrders(
            [FromQuery] DateTime? orderDate,
            [FromQuery] OrderStatus status = OrderStatus.All)
        {
            try
            {
                var count = await _orderService.CountOrders(orderDate ?? DateTime.MinValue, status);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting orders");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
