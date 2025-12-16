using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly POSContext _context;
        public CartRepository(POSContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> AddItemToCartAsync(Guid userId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
            }

            var product = await _context.Products.FindAsync(productId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (item != null)
            {
                bool isAvailable = product != null && product.UnitsInStock >= (quantity + item.Quantity);
                if (!isAvailable)
                {
                    Console.WriteLine($"Not enough stock for product {productId}. Available: {product.UnitsInStock}, Requested: {quantity + item.Quantity}");
                    return false;
                }
                item.Quantity += quantity;
            }
            else
            {
                bool isAvailable = product != null && product.UnitsInStock >= quantity;
                if (!isAvailable)
                {
                    Console.WriteLine($"Not enough stock for product {productId}. Available: {product.UnitsInStock}, Requested: {quantity}");
                    return false;
                }
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            return true;
        }

        public async Task<Cart?> GetCartByUserId(Guid id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == id);
            if (cart == null)
            {
                cart = new Cart { UserId = id, CreatedDate = DateTime.Now };
                await _context.Carts.AddAsync(cart);
            }
            return cart;
        }

        public async Task<bool> DeleteItem(int itemId)
        {
            var cartItem = _context.CartItems.Find(itemId);
            if (cartItem == null)
            {
                return false; 
            }
            _context.CartItems.Remove(cartItem);
            return true;
        }

        public async Task<bool> DeleteAllItemInCart(int cartId)
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CartId == cartId);
            if (cart == null)
            {
                return false; 
            }
            _context.CartItems.RemoveRange(cart.CartItems);
            return true;
        }
    }
}
