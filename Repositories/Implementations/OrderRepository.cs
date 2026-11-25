using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom08LTW.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonHang>> GetAllAsync(string? status = null, string? keyword = null)
        {
            var query = _context.DonHang
                .Include(d => d.TaiKhoan)
                .Include(d => d.ChiTietDonHangs!)
                    .ThenInclude(ct => ct.SanPham)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var aliases = OrderStatusHelper.GetStatusAliases(status);
                query = query.Where(d => aliases.Contains(d.TrangThai));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmed = keyword.Trim();
                query = query.Where(d =>
                    d.MaDonHang.Contains(trimmed) ||
                    d.TenNguoiNhan.Contains(trimmed) ||
                    d.SoDienThoai.Contains(trimmed) ||
                    (d.TaiKhoan != null && (d.TaiKhoan.HoTen.Contains(trimmed) || d.TaiKhoan.Email.Contains(trimmed))));
            }

            return await query
                .OrderByDescending(d => d.NgayDat)
                .ThenByDescending(d => d.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonHang>> GetByUserIdAsync(int userId)
        {
            return await _context.DonHang
                .Include(d => d.ChiTietDonHangs!)
                    .ThenInclude(ct => ct.SanPham)
                .Where(d => d.MaTaiKhoanId == userId)
                .OrderByDescending(d => d.NgayDat)
                .ThenByDescending(d => d.Id)
                .ToListAsync();
        }

        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _context.DonHang.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DonHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DonHang
                .Include(d => d.TaiKhoan)
                .Include(d => d.ChiTietDonHangs!)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DonHang>> GetRecentOrdersAsync(int count = 5)
        {
            return await _context.DonHang
                .Include(d => d.TaiKhoan)
                .OrderByDescending(d => d.NgayDat)
                .ThenByDescending(d => d.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.DonHang.CountAsync();
        }

        public async Task<int> CountByStatusAsync(string status)
        {
            var aliases = OrderStatusHelper.GetStatusAliases(status);
            return await _context.DonHang.CountAsync(d => aliases.Contains(d.TrangThai));
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var cancelledStatuses = OrderStatusHelper.GetStatusAliases(OrderStatusHelper.DaHuy);
            return await _context.DonHang
                .Where(d => !cancelledStatuses.Contains(d.TrangThai))
                .SumAsync(d => (decimal?)d.TongTien) ?? 0m;
        }

        public async Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int year)
        {
            var cancelledStatuses = OrderStatusHelper.GetStatusAliases(OrderStatusHelper.DaHuy);
            var grouped = await _context.DonHang
                .Where(d => d.NgayDat.Year == year && !cancelledStatuses.Contains(d.TrangThai))
                .GroupBy(d => d.NgayDat.Month)
                .Select(g => new MonthlyRevenueItem
                {
                    Thang = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien),
                    SoDonHang = g.Count()
                })
                .ToListAsync();

            var lookup = grouped.ToDictionary(x => x.Thang);
            var result = new List<MonthlyRevenueItem>(12);
            for (int month = 1; month <= 12; month++)
            {
                if (lookup.TryGetValue(month, out var item))
                {
                    result.Add(item);
                }
                else
                {
                    result.Add(new MonthlyRevenueItem
                    {
                        Thang = month,
                        DoanhThu = 0m,
                        SoDonHang = 0
                    });
                }
            }

            return result;
        }

        public async Task<List<int>> GetAvailableOrderYearsAsync()
        {
            var years = await _context.DonHang
                .Select(d => d.NgayDat.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            if (!years.Contains(DateTime.Now.Year))
            {
                years.Insert(0, DateTime.Now.Year);
                years = years.OrderByDescending(y => y).ToList();
            }

            return years;
        }

        public async Task UpdateAsync(DonHang donHang)
        {
            _context.DonHang.Update(donHang);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
