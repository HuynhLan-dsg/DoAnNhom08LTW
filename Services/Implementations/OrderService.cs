using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<DonHang>> GetAllOrdersAsync(string? status = null, string? keyword = null)
        {
            return await _orderRepository.GetAllAsync(status, keyword);
        }

        public async Task<IEnumerable<DonHang>> GetOrdersByUserAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<DonHang?> GetOrderDetailAsync(int id)
        {
            return await _orderRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<(bool success, string message)> ConfirmOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng");
            }

            if (!OrderStatusHelper.CanConfirm(order.TrangThai))
            {
                return (false, $"Chỉ xác nhận đơn hàng đang ở trạng thái \"Chờ xác nhận\" (hiện tại: {OrderStatusHelper.GetDisplayName(order.TrangThai)})");
            }

            order.TrangThai = OrderStatusHelper.FormatForStorage(order.TrangThai, OrderStatusHelper.DaXacNhan);
            await _orderRepository.UpdateAsync(order);

            return (true, $"Đã xác nhận đơn hàng {order.MaDonHang} thành công!");
        }

        public async Task<(bool success, string message)> CancelOrderAsync(int id, string? reason = null)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng");
            }

            if (!OrderStatusHelper.CanCancel(order.TrangThai))
            {
                return (false, $"Không thể hủy đơn hàng {order.MaDonHang} ở trạng thái \"{OrderStatusHelper.GetDisplayName(order.TrangThai)}\".");
            }

            RestoreStock(order);
            order.TrangThai = OrderStatusHelper.FormatForStorage(order.TrangThai, OrderStatusHelper.DaHuy);

            if (!string.IsNullOrWhiteSpace(reason))
            {
                var cancelNote = $"[Hủy đơn: {reason.Trim()}]";
                order.GhiChu = string.IsNullOrWhiteSpace(order.GhiChu)
                    ? cancelNote
                    : $"{order.GhiChu} | {cancelNote}";

                if (order.GhiChu.Length > 500)
                {
                    order.GhiChu = order.GhiChu[..500];
                }
            }

            await _orderRepository.UpdateAsync(order);
            return (true, $"Đã hủy đơn hàng {order.MaDonHang} và hoàn lại tồn kho thành công");
        }

        public async Task<(bool success, string message)> UpdateDeliveryStatusAsync(int id, string newStatus)
        {
            var normalizedTarget = OrderStatusHelper.Normalize(newStatus);
            if (!OrderStatusHelper.AllStatuses.Contains(normalizedTarget))
            {
                return (false, "Trạng thái đơn hàng không hợp lệ");
            }

            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng");
            }

            if (!OrderStatusHelper.IsValidTransition(order.TrangThai, normalizedTarget))
            {
                return (false, $"Không thể chuyển trạng thái đơn hàng từ \"{OrderStatusHelper.GetDisplayName(order.TrangThai)}\" sang \"{OrderStatusHelper.GetDisplayName(normalizedTarget)}\".");
            }

            if (normalizedTarget == OrderStatusHelper.DaHuy)
            {
                RestoreStock(order);
            }

            order.TrangThai = OrderStatusHelper.FormatForStorage(order.TrangThai, normalizedTarget);
            await _orderRepository.UpdateAsync(order);

            return (true, $"Đã cập nhật trạng thái đơn hàng {order.MaDonHang} thành \"{OrderStatusHelper.GetDisplayName(normalizedTarget)}\"!");
        }

        private static void RestoreStock(DonHang order)
        {
            if (order.ChiTietDonHangs == null) return;

            foreach (var detail in order.ChiTietDonHangs)
            {
                if (detail.SanPham != null && detail.SoLuong > 0)
                {
                    detail.SanPham.SoLuongTon += detail.SoLuong;
                }
            }
        }
    }
}
