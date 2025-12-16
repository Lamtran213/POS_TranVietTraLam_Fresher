namespace POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO
{
    public class JwtSettings
    {
        public string AccessSecretKey { get; set; }
        public string RefreshSecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessExpiration { get; set; }
        public int RefreshExpiration { get; set; }
    }
}
