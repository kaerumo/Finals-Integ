using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FinalsInteg.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        string role;
        if (model.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) && model.Password == "admin123")
            role = "Admin";
        else if (model.Username.Equals("customer", StringComparison.OrdinalIgnoreCase) && model.Password == "customer123")
            role = "Customer";
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, model.Username),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "StockFlowCookie");
        await HttpContext.SignInAsync("StockFlowCookie", new ClaimsPrincipal(identity));

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToAction(role == "Admin" ? "Index" : "Index", role == "Admin" ? "Home" : "Customer");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("StockFlowCookie");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();
}

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
