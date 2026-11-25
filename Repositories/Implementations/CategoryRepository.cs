using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom08LTW.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DanhMuc>> GetAllAsync(string? keyword = null, bool? isActive = null)
        {
            var query = _context.DanhMuc
                .Include(d => d.SanPhams)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmed = keyword.Trim();
                query = query.Where(d => d.TenDanhMuc.Contains(trimmed) || d.MaDanhMuc.Contains(trimmed));
            }

            if (isActive.HasValue)
            {
                query = query.Where(d => d.DangHoatDong == isActive.Value);
            }

            return await query.OrderBy(d => d.Id).ToListAsync();
        }

        public async Task<IEnumerable<DanhMuc>> GetActiveAsync()
        {
            return await _context.DanhMuc
                .Where(d => d.DangHoatDong)
                .OrderBy(d => d.TenDanhMuc)
                .ToListAsync();
        }

        public async Task<DanhMuc?> GetByIdAsync(int id)
        {
            return await _context.DanhMuc
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string tenDanhMuc, int? excludeId = null)
        {
            var normalized = tenDanhMuc.Trim().ToLower();
            return await _context.DanhMuc.AnyAsync(d =>
                d.TenDanhMuc.ToLower() == normalized &&
                (!excludeId.HasValue || d.Id != excludeId.Value));
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _context.SanPham.AnyAsync(p => p.MaDanhMucId == id);
        }

        public async Task AddAsync(DanhMuc danhMuc)
        {
            await _context.DanhMuc.AddAsync(danhMuc);
        }

        public async Task UpdateAsync(DanhMuc danhMuc)
        {
            _context.DanhMuc.Update(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DanhMuc danhMuc)
        {
            _context.DanhMuc.Remove(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
