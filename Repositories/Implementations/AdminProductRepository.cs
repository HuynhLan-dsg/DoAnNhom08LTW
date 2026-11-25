using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom08LTW.Repositories.Implementations
{
    public class AdminProductRepository : IAdminProductRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SanPham>> GetAllAsync(
            string? keyword = null,
            int? categoryId = null,
            bool? dangBan = null,
            string? stockFilter = null)
        {
            var query = _context.SanPham
                .Include(p => p.DanhMuc)
                .Include(p => p.KhuyenMai)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmed = keyword.Trim();
                query = query.Where(p =>
                    p.TenSanPham.Contains(trimmed) ||
                    p.MaSanPham.Contains(trimmed) ||
                    p.ThuongHieu.Contains(trimmed));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.MaDanhMucId == categoryId.Value);
            }

            if (dangBan.HasValue)
            {
                query = query.Where(p => p.DangBan == dangBan.Value);
            }

            if (!string.IsNullOrWhiteSpace(stockFilter))
            {
                query = stockFilter.ToLowerInvariant() switch
                {
                    "out" => query.Where(p => p.SoLuongTon == 0),
                    "low" => query.Where(p => p.SoLuongTon > 0 && p.SoLuongTon <= 10),
                    "in" => query.Where(p => p.SoLuongTon > 10),
                    _ => query
                };
            }

            return await query
                .OrderByDescending(p => p.DangBan)
                .ThenByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<SanPham?> GetByIdAsync(int id)
        {
            return await _context.SanPham
                .Include(p => p.DanhMuc)
                .Include(p => p.KhuyenMai)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> CountAllAsync()
        {
            return await _context.SanPham.CountAsync();
        }

        public async Task<int> CountActiveAsync()
        {
            return await _context.SanPham.CountAsync(p => p.DangBan);
        }

        public async Task<int> CountLowStockAsync(int threshold = 10)
        {
            return await _context.SanPham.CountAsync(p => p.DangBan && p.SoLuongTon <= threshold);
        }

        public async Task<IEnumerable<SanPham>> GetLowStockProductsAsync(int threshold = 10, int take = 5)
        {
            return await _context.SanPham
                .Include(p => p.DanhMuc)
                .Where(p => p.DangBan && p.SoLuongTon <= threshold)
                .OrderBy(p => p.SoLuongTon)
                .ThenBy(p => p.Id)
                .Take(take)
                .ToListAsync();
        }

        public async Task AddAsync(SanPham sanPham)
        {
            await _context.SanPham.AddAsync(sanPham);
        }

        public async Task UpdateAsync(SanPham sanPham)
        {
            _context.SanPham.Update(sanPham);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
