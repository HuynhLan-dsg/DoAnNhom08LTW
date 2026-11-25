using System.Text.Json;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private const string CartSessionKey = "Cart";

        public IViewComponentResult Invoke()
        {
            var model = new CartSummaryViewModel();
            var rawJson = ReadCartJson();

            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(rawJson);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in doc.RootElement.EnumerateArray())
                        {
                            int qty = TryReadInt(element, "SoLuong", "soLuong", "Quantity", "quantity") ?? 1;
                            decimal price = TryReadDecimal(element, "DonGia", "donGia", "GiaBan", "giaBan", "Price", "price") ?? 0m;

                            if (qty > 0)
                            {
                                model.TotalQuantity += qty;
                                model.TotalAmount += qty * price;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return View(model);
        }

        private string? ReadCartJson()
        {
            var sessionFeature = HttpContext.Features.Get<ISessionFeature>();
            if (sessionFeature?.Session != null)
            {
                var sessionData = sessionFeature.Session.GetString(CartSessionKey);
                if (!string.IsNullOrWhiteSpace(sessionData))
                {
                    return sessionData;
                }
            }

            if (HttpContext.Request.Cookies.TryGetValue(CartSessionKey, out var cookieData) &&
                !string.IsNullOrWhiteSpace(cookieData))
            {
                return cookieData;
            }

            if (HttpContext.Request.Cookies.TryGetValue("cart", out var lowerCookieData) &&
                !string.IsNullOrWhiteSpace(lowerCookieData))
            {
                return lowerCookieData;
            }

            return null;
        }

        private static int? TryReadInt(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out int val))
                        return val;
                    if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out int parsed))
                        return parsed;
                }
            }

            return null;
        }

        private static decimal? TryReadDecimal(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDecimal(out decimal val))
                        return val;
                    if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out decimal parsed))
                        return parsed;
                }
            }

            return null;
        }
    }
}
