namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IOTPRepository OTPRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        Task<bool> Save();
    }
}
