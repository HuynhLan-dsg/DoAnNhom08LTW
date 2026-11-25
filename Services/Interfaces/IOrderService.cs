using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<DonHang>> GetAllOrdersAsync(string? status = null, string? keyword = null);
        Task<IEnumerable<DonHang>> GetOrdersByUserAsync(int userId);
        Task<DonHang?> GetOrderDetailAsync(int id);
        Task<(bool success, string message)> ConfirmOrderAsync(int id);
        Task<(bool success, string message)> CancelOrderAsync(int id, string? reason = null);
        Task<(bool success, string message)> UpdateDeliveryStatusAsync(int id, string newStatus);
    }
}
