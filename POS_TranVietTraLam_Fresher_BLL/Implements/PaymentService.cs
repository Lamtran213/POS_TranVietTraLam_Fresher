using Microsoft.AspNetCore.SignalR;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO;
using POS_TranVietTraLam_Fresher_BLL.Hubs;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHubContext<POSHubs> _hubContext;
        public PaymentService(IUnitOfWork unitOfWork, IEmailService emailService, IHubContext<POSHubs> hubContext)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hubContext = hubContext;
        }
        public async Task<bool> HandlePayOSWebhook(PayosWebhookPayload payload)
        {
            // PayOS gửi orderCode trong data.orderCode, không phải payload.orderCode
            var actualOrderCode = payload.data?.orderCode ?? payload.orderCode;

            // Log webhook payload for debugging
            Console.WriteLine($"PayOS Webhook received - OrderCode: {actualOrderCode}, Status: {payload.status}, Code: {payload.code}, Desc: {payload.desc}");
            Console.WriteLine($"Data orderCode: {payload.data?.orderCode}, Success: {payload.success}");

            if (actualOrderCode == 0)
            {
                Console.WriteLine("OrderCode is 0, skipping processing");
                return true;
            }

            var payment = await _unitOfWork.PaymentRepository.GetByOrderCodeAsync(actualOrderCode);
            var order = payment != null ? await _unitOfWork.OrderRepository.GetByIdAsync(payment.OrderId) : null;
            if (payment == null)
            {
                return true;
            }

            PaymentStatus newStatus;

            // PayOS dùng success field và code/desc để xác định trạng thái
            if (payload.success == true && payload.code == "00" && payload.desc?.ToLowerInvariant().Contains("success") == true)
            {
                newStatus = PaymentStatus.Paid;
            }
            else if (payload.success == false || payload.code != "00" || payload.desc?.ToLowerInvariant().Contains("success") != true)
            {
                newStatus = PaymentStatus.Failed;
            }
            else
            {
                Console.WriteLine($"Cannot determine status from payload: success={payload.success}, code={payload.code}, desc={payload.desc}");
                return false;
            }

            Console.WriteLine($"Processing status change: {payment.Status} -> {newStatus}");

            if (payment.Status == newStatus)
            {
                Console.WriteLine($"Status unchanged, skipping processing");
                return true;
            }

            if (newStatus == PaymentStatus.Paid)
            {
                var paidAt = payload.paidAt.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(payload.paidAt.Value)
                    : DateTimeOffset.UtcNow;

                var orders = await _unitOfWork.OrderDetailRepository.GetOrderWithDetailsAsync(payment.OrderId);
                if(orders == null)
                {
                    Console.WriteLine($"Order not found for PaymentId: {payment.PaymentId}");
                    return false;
                }

                foreach(var detail in orders.OrderDetails)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(detail.ProductId);
                    if(product == null)
                    {
                        Console.WriteLine($"Product not found for ProductId: {detail.ProductId}");
                        return false;
                    }

                    if(product.UnitsInStock < detail.Quantity)
                    {
                        Console.WriteLine($"Insufficient stock for ProductId: {detail.ProductId}. Available: {product.UnitsInStock}, Required: {detail.Quantity}");
                        return false;
                    }

                    product.UnitsInStock -= detail.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }

                await _unitOfWork.PaymentRepository.MarkPaidAsync(payment.PaymentId, paidAt);
                await _unitOfWork.OrderRepository.MarkPaidAsync(payment.OrderId, paidAt);
                await NotifyPaymentChangedAsync();

                await _unitOfWork.Save();

                // Gửi email xác nhận thanh toán thành công
                var user = await _unitOfWork.UserRepository.GetByIdAsync(payment.UserId);
                if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                {
                    var subject = "Xác nhận thanh toán - POS.Lamtran213";
                    var html = _emailService.BuildPaymentSuccessEmailHtml(
                        user: user,
                        orderCode: payment.PayosOrderCode,
                        amount: payment.Amount,
                        createdAt: payment.CreatedAt,
                        paidAt: paidAt.UtcDateTime
                    );
                    await _emailService.SendEmailAsync(user.Email, subject, html);
                }

                return true;
            }
            else if (newStatus == PaymentStatus.Failed || newStatus == PaymentStatus.Cancelled)
            {
                await _unitOfWork.PaymentRepository.MarkFailedAsync(payment.PaymentId);
                await _unitOfWork.Save();
                await NotifyPaymentChangedAsync();
                return true;
            }

            return false;
        }

        public async Task<List<AllPaymentDTO>> GetAllPaymentAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllWithDetailsAsync();
            var paymentDTOs = payments.Select(p => new AllPaymentDTO
            {
                PaymentId = p.PaymentId,
                PayosOrderCode = p.PayosOrderCode,
                Amount = p.Amount,
                Status = p.Status,
                Method = p.Method,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,
                Email = p.User.Email
            }).ToList();
            return paymentDTOs;
        }

        public async Task NotifyPaymentChangedAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllWithDetailsAsync();

            var paymentDTOs = payments.Select(p => new AllPaymentDTO
            {
                PaymentId = p.PaymentId,
                PayosOrderCode = p.PayosOrderCode,
                Amount = p.Amount,
                Status = p.Status,
                Method = p.Method,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,
                Email = p.User?.Email
            }).ToList();

            await _hubContext.Clients.All.SendAsync(
                "PaymentsUpdated",
                paymentDTOs
            );
        }

    }
}
