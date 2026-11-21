using System.Net;
using System.Text.Json;

namespace DoAnNhom08LTW.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        // Constructor nhận RequestDelegate (luồng đi tiếp theo), ILogger (ghi log) và IHostEnvironment (kiểm tra môi trường Dev/Prod)
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Cho phép request đi tiếp vào các middleware và Controller phía sau
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1. Ghi toàn bộ chi tiết lỗi vào hệ thống Log (Console / Output của Visual Studio)
                _logger.LogError(ex, "Đã xảy ra lỗi hệ thống không mong muốn tại đường dẫn: {Path}", context.Request.Path);

                // 2. Xử lý phản hồi trả về cho người dùng
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Mã lỗi 500

            // Nếu là request gọi API / AJAX (trả về JSON)
            bool isAjaxRequest = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                                 || (context.Request.Headers["Accept"].ToString().Contains("application/json"));

            if (isAjaxRequest)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = _env.IsDevelopment() ? exception.Message : "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                // Nếu là request duyệt web bình thường -> Chuyển hướng về trang báo lỗi /Home/Error
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}