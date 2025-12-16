using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<bool> AddItemToCartAsync(Guid userId, int productId, int quantity);
        Task<Cart?> GetCartByUserId(Guid id);
        Task<bool> DeleteItem(int itemId);
        Task<bool> DeleteAllItemInCart(int cartId);
    }
}
