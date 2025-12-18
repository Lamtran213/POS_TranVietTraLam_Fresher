using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IQueryable<Product> Query();
        Task<Product> UpdateProductStockAfterPaymentAsync(int productId, int quantityExist);
    }
}
