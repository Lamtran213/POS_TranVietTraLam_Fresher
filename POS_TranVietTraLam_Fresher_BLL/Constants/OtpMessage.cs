namespace POS_TranVietTraLam_Fresher_BLL.Constants
{
    public class OtpMessage
    {
        public const string OtpSentManyTimes = "Bạn đã gửi quá nhiều mã xác thực. Vui lòng thử lại sau 1 giờ.";
        public const string FailedToSendOtp = "Không thể gửi mã xác thực. Vui lòng thử lại.";
        public const string OtpHasBeenSent = "Mã xác thực đã được gửi đến email của bạn.";
        public const string OtpHasBeenErrorForSomeReason = "Đã có lỗi xảy ra khi gửi mã xác thực. Vui lòng thử lại.";
        public const string OtpInvalid = "Mã xác thực không hợp lệ hoặc đã hết hạn.";
        public const string OtpVerifiedSuccess = "Xác thực mã OTP thành công.";
    }
}
