using LMDH_QS.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LMDH_QS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FirstName", user.FirstName),
                    new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                    new Claim("Department", user.Department ?? "")
                };

                var identity = new ClaimsIdentity(claims, "AppScheme");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("AppScheme", principal);

                // 🔁 Role-based redirection
                if (user.Role == "Staff")
                    return RedirectToAction("StaffPageSelector", "Staff");

                if (user.Role == "Doctor")
                    return RedirectToAction("DoctorControl", "Doctor");

                // default fallback (just in case)
                return Redirect(returnUrl);
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AppScheme");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
