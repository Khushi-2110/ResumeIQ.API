using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using ResumeIQ.Web.Models;


namespace ResumeIQ.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();

            // We map "Password" to "PasswordHash" so it matches your API's User model
            var payload = new { Email = model.Email, PasswordHash = model.Password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7202/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Email) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();

            var payload = new
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password,
                Role = "job_seeker"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7202/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                // Registration worked! Send them to Login.
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed. Email might already exist.";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

}
