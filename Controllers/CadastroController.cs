using Microsoft.AspNetCore.Mvc;
using ViziLogin.Data;
using ViziLogin.Models;
using Microsoft.EntityFrameworkCore;

public class CadastroController : Controller
{
    private readonly AppDbContext _context;

    public CadastroController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Cadastro
    public IActionResult Index()
    {
        // Evita crash se a tabela Areas não existir
        try
        {
            ViewBag.Areas = _context.Areas?.ToList();
        }
        catch
        {
            ViewBag.Areas = null;
        }

        return View();
    }

    // POST: Cadastro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(Usuario usuario, string ConfirmarSenha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome) || !usuario.Nome.Trim().Contains(" "))
            {
                ViewBag.Error = "Digite seu nome completo (Nome e Sobrenome).";

                try { ViewBag.Areas = _context.Areas?.ToList(); } catch { ViewBag.Areas = null; }

                return View("Index", usuario);
            }

            usuario.Nome = System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(usuario.Nome.ToLower().Trim());

            if (usuario.Senha != ConfirmarSenha)
            {
                ViewBag.Error = "As senhas não coincidem!";

                try { ViewBag.Areas = _context.Areas?.ToList(); } catch { ViewBag.Areas = null; }

                return View("Index", usuario);
            }

            ModelState.Remove("Id");
            ModelState.Remove("Area");

            if (ModelState.IsValid)
            {
                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email == usuario.Email);

                if (usuarioExiste)
                {
                    ViewBag.Error = "Este e-mail já está cadastrado.";

                    try { ViewBag.Areas = _context.Areas?.ToList(); } catch { ViewBag.Areas = null; }

                    return View("Index", usuario);
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Login");
            }

            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            ViewBag.Error = "Erro nos dados: " + string.Join(", ", erros);

            try { ViewBag.Areas = _context.Areas?.ToList(); } catch { ViewBag.Areas = null; }

            return View("Index", usuario);
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Erro inesperado: " + ex.Message;
            return View("Index", usuario);
        }
    }
}