using POS_TranVietTraLam_Fresher_Entities.Enum;

public class CreateOrderResponseDTO
{
    public int OrderId { get; set; }

    /// <summary>
    /// Chỉ có khi PaymentMethod = PayOS
    /// </summary>
    public string? PaymentUrl { get; set; }
    public string? RedirectUrl { get; set; }

    public bool IsPaid { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
