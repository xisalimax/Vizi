using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;
using System.Globalization;

public class CadastroController : Controller
{
    private readonly AppDbContext _context;

    public CadastroController(AppDbContext context)
    {
        _context = context;
    }

    // 🔹 Carrega Areas sempre que precisar
    private void CarregarAreas()
    {
        ViewBag.Areas = _context.Areas.ToList();
    }

    // GET: Cadastro
    public IActionResult Index()
    {
        CarregarAreas();
        return View();
    }

    // POST: Cadastro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(Usuario usuario, string ConfirmarSenha)
    {
        try
        {
            CarregarAreas();

            // Nome completo obrigatório
            if (string.IsNullOrWhiteSpace(usuario.Nome) || !usuario.Nome.Trim().Contains(" "))
            {
                ViewBag.Error = "Digite seu nome completo (Nome e Sobrenome).";
                return View("Index", usuario);
            }

            // Formata nome
            usuario.Nome = CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(usuario.Nome.ToLower().Trim());

            // Confirma senha
            if (usuario.Senha != ConfirmarSenha)
            {
                ViewBag.Error = "As senhas não coincidem!";
                return View("Index", usuario);
            }

            // Limpa validações desnecessárias
            ModelState.Remove("Id");
            ModelState.Remove("Area");

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                ViewBag.Error = "Erro nos dados: " + string.Join(", ", erros);
                return View("Index", usuario);
            }

            // Verifica email duplicado
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == usuario.Email);

            if (usuarioExiste)
            {
                ViewBag.Error = "Este e-mail já está cadastrado.";
                return View("Index", usuario);
            }

            // Salva usuário
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Login");
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Erro inesperado: " + ex.Message;
            CarregarAreas();
            return View("Index", usuario);
        }
    }
}