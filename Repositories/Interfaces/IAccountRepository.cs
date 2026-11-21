using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels;

namespace DoAnNhom08LTW.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<TaiKhoan> GetByEmailAsync(string email);
        Task AddAsync(TaiKhoan taiKhoan);
        Task SaveAsync();
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task<IEnumerable<TaiKhoan>> GetAllAsync();
    }
}
