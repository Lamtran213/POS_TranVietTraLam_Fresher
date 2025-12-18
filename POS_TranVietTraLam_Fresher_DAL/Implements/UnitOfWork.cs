using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly POSContext _context;
        public IUserRepository UserRepository { get; }
        public IOTPRepository OTPRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public ICartRepository CartRepository { get; }
        public ICartItemRepository CartItemRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailRepository OrderDetailRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public UnitOfWork(POSContext context, 
            IUserRepository userRepository,
            IOTPRepository oTPRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            ICartItemRepository cartItemRepository,
            IPaymentRepository paymentRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _context = context;
            UserRepository = userRepository;
            OTPRepository = oTPRepository;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CartRepository = cartRepository;
            OrderRepository = orderRepository;
            CartItemRepository = cartItemRepository;
            PaymentRepository = paymentRepository;
            OrderDetailRepository = orderDetailRepository;
        }
        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
