using DoAnNhom08LTW.Data;
using DoAnNhom08LTW.Middleware;
using DoAnNhom08LTW.Repositories.Implementations;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services;
using DoAnNhom08LTW.Services.Implementations;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data;
using Microsoft.EntityFrameworkCore;
namespace DoAnNhom08LTW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Đăng ký:
            builder.Services.AddScoped<
                IAccountRepository,
                AccountRepository>();

            builder.Services.AddScoped<
                IAccountService,
                AccountService>();
            builder.Services.AddScoped<EmailService>();

            //đăng ký cho DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

            // Authentication
            builder.Services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";               // Chưa đăng nhập -> đẩy về đây
                    options.LogoutPath = "/Account/Logout";             // Đường dẫn đăng xuất
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Đã đăng nhập nhưng sai quyền -> đẩy về đây
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);     // Thời gian sống của Cookie
                });
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession();
            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();


            
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
