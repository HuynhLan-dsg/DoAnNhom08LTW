namespace DoAnNhom08LTW.Models.ViewModels.Admin
{
    public static class OrderStatusHelper
    {
        public const string ChoXacNhan = "ChoXacNhan";
        public const string DaXacNhan = "DaXacNhan";
        public const string DangGiao = "DangGiao";
        public const string DaGiao = "DaGiao";
        public const string DaHuy = "DaHuy";

        public static readonly string[] AllStatuses =
        {
            ChoXacNhan,
            DaXacNhan,
            DangGiao,
            DaGiao,
            DaHuy
        };

        public static string Normalize(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return ChoXacNhan;

            var trimmed = status.Trim();
            return trimmed.ToLowerInvariant() switch
            {
                "choxacnhan" or "chờ xác nhận" => ChoXacNhan,
                "daxacnhan" or "đã xác nhận" => DaXacNhan,
                "danggiao" or "đang giao" or "đang giao hàng" => DangGiao,
                "dagiao" or "đã giao" or "hoàn thành" => DaGiao,
                "dahuy" or "đã hủy" or "hủy" => DaHuy,
                _ => trimmed
            };
        }

        public static string[] GetStatusAliases(string? status)
        {
            return Normalize(status) switch
            {
                ChoXacNhan => new[] { ChoXacNhan, "Chờ xác nhận" },
                DaXacNhan => new[] { DaXacNhan, "Đã xác nhận" },
                DangGiao => new[] { DangGiao, "Đang giao", "Đang giao hàng" },
                DaGiao => new[] { DaGiao, "Đã giao", "Hoàn thành" },
                DaHuy => new[] { DaHuy, "Đã hủy", "Hủy" },
                _ => string.IsNullOrWhiteSpace(status) ? Array.Empty<string>() : new[] { status.Trim() }
            };
        }

        public static string FormatForStorage(string? existingStatus, string normalizedTarget)
        {
            var target = Normalize(normalizedTarget);
            if (!string.IsNullOrWhiteSpace(existingStatus))
            {
                var trimmed = existingStatus.Trim();
                bool usesVietnameseLabel = trimmed.Contains(' ') || trimmed.Any(c => c > 127);
                if (usesVietnameseLabel)
                {
                    return target switch
                    {
                        ChoXacNhan => "Chờ xác nhận",
                        DaXacNhan => "Đã xác nhận",
                        DangGiao => trimmed.Equals("Đang giao hàng", StringComparison.OrdinalIgnoreCase) ? "Đang giao hàng" : "Đang giao",
                        DaGiao => "Đã giao",
                        DaHuy => "Đã hủy",
                        _ => target
                    };
                }
            }

            return target;
        }

        public static string GetDisplayName(string? status)
        {
            return Normalize(status) switch
            {
                ChoXacNhan => "Chờ xác nhận",
                DaXacNhan => "Đã xác nhận",
                DangGiao => "Đang giao hàng",
                DaGiao => "Đã giao",
                DaHuy => "Đã hủy",
                _ => status ?? "Không xác định"
            };
        }

        public static string GetBadgeClass(string? status)
        {
            return Normalize(status) switch
            {
                ChoXacNhan => "bg-warning text-dark",
                DaXacNhan => "bg-info text-dark",
                DangGiao => "bg-primary",
                DaGiao => "bg-success",
                DaHuy => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public static bool IsCancelled(string? status) => Normalize(status) == DaHuy;

        public static bool CanConfirm(string? status) => Normalize(status) == ChoXacNhan;

        public static bool CanShip(string? status) => Normalize(status) == DaXacNhan;

        public static bool CanComplete(string? status) => Normalize(status) == DangGiao;

        public static bool CanCancel(string? status)
        {
            var normalized = Normalize(status);
            return normalized == ChoXacNhan || normalized == DaXacNhan || normalized == DangGiao;
        }

        public static bool IsValidTransition(string? currentStatus, string? targetStatus)
        {
            var current = Normalize(currentStatus);
            var target = Normalize(targetStatus);

            if (current == target) return false;

            return (current, target) switch
            {
                (ChoXacNhan, DaXacNhan) => true,
                (ChoXacNhan, DaHuy) => true,
                (DaXacNhan, DangGiao) => true,
                (DaXacNhan, DaHuy) => true,
                (DangGiao, DaGiao) => true,
                (DangGiao, DaHuy) => true,
                _ => false
            };
        }
    }
}
