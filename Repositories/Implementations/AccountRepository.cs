using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace DoAnNhom08LTW.Repositories.Implementations
{
    public class AccountRepository: IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaiKhoan> GetByEmailAsync(string email)
        {
            return await _context.TaiKhoan.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            await _context.TaiKhoan.AddAsync(taiKhoan);
        }
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _context.TaiKhoan.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            _context.TaiKhoan.Update(taiKhoan);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TaiKhoan>> GetAllAsync()
        {
            return await _context.TaiKhoan
                .OrderBy(t => t.Id)
                .ToListAsync();
        }
    }
}
