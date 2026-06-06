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

    public IActionResult Index()
    {
        ViewBag.Areas = _context.Areas.ToList(); // ← carrega as regionais
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(Usuario usuario, string ConfirmarSenha)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome) || !usuario.Nome.Trim().Contains(" "))
        {
            ViewBag.Error = "Digite seu nome completo (Nome e Sobrenome).";
            ViewBag.Areas = _context.Areas.ToList(); // ← recarrega se der erro
            return View("Index", usuario);
        }

        usuario.Nome = System.Globalization.CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(usuario.Nome.ToLower().Trim());

        if (usuario.Senha != ConfirmarSenha)
        {
            ViewBag.Error = "As senhas não coincidem!";
            ViewBag.Areas = _context.Areas.ToList(); // ← recarrega se der erro
            return View("Index", usuario);
        }

        ModelState.Remove("Id");
        ModelState.Remove("Area"); // ← ignora validação do objeto Area

        if (ModelState.IsValid)
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
            if (usuarioExiste)
            {
                ViewBag.Error = "Este e-mail já está cadastrado.";
                ViewBag.Areas = _context.Areas.ToList(); // ← recarrega se der erro
                return View("Index", usuario);
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Login");
        }

        var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        ViewBag.Error = "Erro nos dados: " + string.Join(", ", erros);
        ViewBag.Areas = _context.Areas.ToList(); // ← recarrega se der erro

        return View("Index", usuario);
    }
}