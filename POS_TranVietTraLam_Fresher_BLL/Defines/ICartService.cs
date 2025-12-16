using POS_TranVietTraLam_Fresher_BLL.DTO.CartDTO;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface ICartService
    {
        Task<bool> AddItemToCartAsync(AddToCartRequestDTO dto);
        Task<bool> CreateNewCart(Guid userId);
        Task<CartDTO?> GetByUserId(Guid userId);
        Task<bool> UpdateCartItemQuantity(AddToCartRequestDTO dto);
        Task<bool> DeleteCartItem(Guid userId, int itemId);
    }
}
