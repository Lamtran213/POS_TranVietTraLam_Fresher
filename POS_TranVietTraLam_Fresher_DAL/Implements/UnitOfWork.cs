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
        public UnitOfWork(POSContext context, 
            IUserRepository userRepository,
            IOTPRepository oTPRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository)
        {
            _context = context;
            UserRepository = userRepository;
            OTPRepository = oTPRepository;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CartRepository = cartRepository;
        }
        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
