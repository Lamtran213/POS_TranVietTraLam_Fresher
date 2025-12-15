namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IOTPRepository OTPRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task<bool> Save();
    }
}
