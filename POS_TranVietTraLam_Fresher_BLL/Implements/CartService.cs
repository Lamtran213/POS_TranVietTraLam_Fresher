using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CartDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.CartItemDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticatedUser _authenticatedUser;
        public CartService(IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser)
        {
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
        }

        public async Task<bool> AddItemToCartAsync(AddToCartRequestDTO dto)
        {
            bool firstCondition = await _unitOfWork.CartRepository.AddItemToCartAsync(_authenticatedUser.UserId, dto.ProductId, dto.Quantity);
            bool secondCondition = await _unitOfWork.Save();
            return firstCondition && secondCondition;
        }

        public async Task<bool> CreateNewCart(Guid userId)
        {
            await _unitOfWork.CartRepository.AddAsync(new Cart
            {
                UserId = userId,
                CreatedDate = DateTime.Now
            });
            return await _unitOfWork.Save();
        }

        public async Task<CartDTO?> GetByUserId(Guid userId)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByUserId(userId);
            if (cart is null)
                return null;

            return new CartDTO
            {
                CartId = cart.CartId,
                MemberId = cart.UserId,
                CartItems = cart.CartItems.Select(item => new CartItemsDTO
                {
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice * (decimal)(1 - item.Product.Discount),
                    ImageUrl = item.Product.ImageUrl,
                }).ToList(),
            };
        }

        public async Task<bool> UpdateCartItemQuantity(AddToCartRequestDTO dto)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByUserId(_authenticatedUser.UserId);
            if (cart == null)
            {
                return false; // Cart not found
            }

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == dto.ProductId);
            if (dto.Quantity > product.UnitsInStock)
                return false; // Not enough stock available

            if (cartItem != null)
            {
                cartItem.Quantity = dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            return await _unitOfWork.Save();
        }

        public async Task<bool> DeleteCartItem(Guid userId, int itemId)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByUserId(userId);
            if (cart == null)
            {
                return false; 
            }

            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == itemId);

            return await _unitOfWork.CartRepository.DeleteItem(cartItem.CartItemId) && await _unitOfWork.Save();
        }
    }
}
