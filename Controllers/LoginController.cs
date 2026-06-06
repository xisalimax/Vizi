using ViziLogin.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ViziLogin.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string senha)
        {
            var user = _context.Usuarios
                .FirstOrDefault(x => x.Email == email && x.Senha == senha);

            if (user == null)
            {
                ViewBag.Error = "Dados inválidos ou incorretos";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Tipo_Perfil ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            // ✅ Admin vai para painel exclusivo, demais vão para Home
            if (user.Tipo_Perfil == "ADMINISTRADOR")
                return RedirectToAction("Index", "Admin");

            TempData["Success"] = "Transação OK, você está logado";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
