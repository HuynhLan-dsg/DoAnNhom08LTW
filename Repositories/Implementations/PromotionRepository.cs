using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom08LTW.Repositories.Implementations
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDbContext _context;

        public PromotionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KhuyenMai>> GetAllAsync(string? keyword = null, bool? trangThai = null)
        {
            var query = _context.KhuyenMai
                .Include(k => k.SanPhams)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmed = keyword.Trim();
                query = query.Where(k =>
                    k.TenKhuyenMai.Contains(trimmed) ||
                    k.MaKhuyenMai.Contains(trimmed));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(k => k.TrangThai == trangThai.Value);
            }

            return await query.OrderByDescending(k => k.Id).ToListAsync();
        }

        public async Task<IEnumerable<KhuyenMai>> GetActiveAsync()
        {
            return await _context.KhuyenMai
                .Where(k => k.TrangThai)
                .OrderByDescending(k => k.Id)
                .ToListAsync();
        }

        public async Task<KhuyenMai?> GetByIdAsync(int id)
        {
            return await _context.KhuyenMai
                .Include(k => k.SanPhams)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<int> CountExpiringSoonAsync(int days = 7)
        {
            var now = DateTime.Now;
            var limit = now.AddDays(days);
            return await _context.KhuyenMai.CountAsync(k =>
                k.TrangThai && k.NgayKetThuc >= now && k.NgayKetThuc <= limit);
        }

        public async Task AddAsync(KhuyenMai khuyenMai)
        {
            await _context.KhuyenMai.AddAsync(khuyenMai);
        }

        public async Task UpdateAsync(KhuyenMai khuyenMai)
        {
            _context.KhuyenMai.Update(khuyenMai);
            await _context.SaveChangesAsync();
        }

        public async Task DetachProductsAsync(int promotionId)
        {
            var products = await _context.SanPham
                .Where(p => p.MaKhuyenMaiId == promotionId)
                .ToListAsync();

            foreach (var product in products)
            {
                product.MaKhuyenMaiId = null;
            }

            if (products.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(KhuyenMai khuyenMai)
        {
            _context.KhuyenMai.Remove(khuyenMai);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
