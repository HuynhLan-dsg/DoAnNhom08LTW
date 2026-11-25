using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminProductRepository _productRepository;
        private readonly IPromotionRepository _promotionRepository;

        public NotificationViewComponent(
            IOrderRepository orderRepository,
            IAdminProductRepository productRepository,
            IPromotionRepository promotionRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pendingOrders = await _orderRepository.CountByStatusAsync(OrderStatusHelper.ChoXacNhan);
            var lowStockCount = await _productRepository.CountLowStockAsync(10);
            var expiringPromos = await _promotionRepository.CountExpiringSoonAsync(7);

            var items = new List<NotificationItemViewModel>();

            if (pendingOrders > 0)
            {
                items.Add(new NotificationItemViewModel
                {
                    Title = "Đơn hàng chờ xác nhận",
                    Message = $"Hiện có {pendingOrders} đơn hàng mới đang chờ quản trị viên xác nhận.",
                    BadgeText = $"{pendingOrders} đơn",
                    BadgeClass = "bg-warning text-dark",
                    IconClass = "bi-bag-check text-warning",
                    Area = "Admin",
                    Controller = "Order",
                    Action = "Index",
                    RouteValues = new Dictionary<string, string> { ["status"] = OrderStatusHelper.ChoXacNhan }
                });
            }

            if (lowStockCount > 0)
            {
                items.Add(new NotificationItemViewModel
                {
                    Title = "Cảnh báo tồn kho thấp",
                    Message = $"Có {lowStockCount} sản phẩm đang bán có số lượng tồn kho từ 10 trở xuống.",
                    BadgeText = $"{lowStockCount} SP",
                    BadgeClass = "bg-danger",
                    IconClass = "bi-exclamation-triangle text-danger",
                    Area = "Admin",
                    Controller = "Product",
                    Action = "Index",
                    RouteValues = new Dictionary<string, string> { ["stockFilter"] = "low" }
                });
            }

            if (expiringPromos > 0)
            {
                items.Add(new NotificationItemViewModel
                {
                    Title = "Khuyến mãi sắp hết hạn",
                    Message = $"Có {expiringPromos} chương trình khuyến mãi sẽ kết thúc trong 7 ngày tới.",
                    BadgeText = $"{expiringPromos} KM",
                    BadgeClass = "bg-info text-dark",
                    IconClass = "bi-clock-history text-info",
                    Area = "Admin",
                    Controller = "Promotion",
                    Action = "Index"
                });
            }

            var model = new NotificationViewModel
            {
                PendingOrderCount = pendingOrders,
                LowStockProductCount = lowStockCount,
                ExpiringPromotionCount = expiringPromos,
                Items = items
            };

            return View(model);
        }
    }
}
