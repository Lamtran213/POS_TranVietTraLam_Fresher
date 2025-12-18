using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly POSContext _context;
        public ProductRepository(POSContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Product> Query()
        {
            return _context.Products.AsQueryable();
        }

        public async Task<Product> UpdateProductStockAfterPaymentAsync(int productId, int quantityExist)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.UnitsInStock = quantityExist;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return product;
        }
    }
}
