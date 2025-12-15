namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IOTPRepository OTPRepository { get; }
        Task<bool> Save();
    }
}
