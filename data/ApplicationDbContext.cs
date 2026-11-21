using DoAnNhom08LTW.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DoAnNhom08LTW.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<DanhMuc> DanhMuc { get; set; }
        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<DonHang> DonHang { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Bảng TaiKhoan: MaTaiKhoan là cột tự tính toán (PERSISTED)
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.Property(e => e.MaTaiKhoan)
                    .HasComputedColumnSql("('TK' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);

            });

            // 2. Bảng DanhMuc: MaDanhMuc là cột tự tính toán
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.Property(e => e.MaDanhMuc)
                    .HasComputedColumnSql("('DM' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);

            });

            // 3. Bảng KhuyenMai: MaKhuyenMai là cột tự tính toán
            modelBuilder.Entity<KhuyenMai>(entity =>
            {
                entity.Property(e => e.MaKhuyenMai)
                    .HasComputedColumnSql("('KM' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);

            });

            // 4. Bảng SanPham: MaSanPham là cột tự tính toán
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.Property(e => e.MaSanPham)
                    .HasComputedColumnSql("('SP' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);

            });

            // 5. Bảng DonHang: MaDonHang là cột tự tính toán
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.Property(e => e.MaDonHang)
                    .HasComputedColumnSql("('DH' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);

            });

            // 6. Bảng ChiTietDonHang: MaChiTiet và ThanhTien là cột tự tính toán
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.Property(e => e.MaChiTiet)
                    .HasComputedColumnSql("('CT' + CASE WHEN [Id] < 10000 THEN RIGHT('0000' + CONVERT(VARCHAR(20), [Id]), 4) ELSE CONVERT(VARCHAR(20), [Id]) END)", stored: true);



                entity.Property(e => e.ThanhTien)
                    .HasComputedColumnSql("(CONVERT(DECIMAL(18,2), [SoLuong] * [DonGia]))", stored: true);
            });
        }
    }
}