using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CartDTO;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private IAuthenticatedUser _authenticatedUser;

        public CartController(ICartService cartService, IAuthenticatedUser authenticatedUser)
        {
            _cartService = cartService;
            _authenticatedUser = authenticatedUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartByUserId()
        {
            var currentUserId = _authenticatedUser.UserId;
            var cart = await _cartService.GetByUserId(currentUserId);
            return Ok(cart);
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddToCartRequestDTO dto)
        {
            var result = await _cartService.AddItemToCartAsync(dto);
            return Ok(new
            {
                IsSuccess = result
            });
        }

        [HttpPost("create-cart/{userId}")]
        public async Task<IActionResult> CreateNewCart(Guid userId)
        {
            var result = await _cartService.CreateNewCart(userId);
            return Ok(new
            {
                IsSuccess = result
            });
        }

        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] AddToCartRequestDTO dto)
        {
            var result = await _cartService.UpdateCartItemQuantity(dto);
            return Ok(new
            {
                IsSuccess = result
            });
        }

        [HttpDelete("remove-item/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            var result = await _cartService.DeleteCartItem(_authenticatedUser.UserId, productId);
            return Ok(new
            {
                IsSuccess = result
            });
        }
    }
}
