using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ViziLogin.Data;
using ViziLogin.Models;

namespace ViziLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string tipo, int pagina = 1, bool verTodas = false)
        {
            var emailLogado = User.Identity.Name;
            var usuarioLogado = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == emailLogado);

            IQueryable<Servico> consulta = _context.Servicos
                .Include(s => s.Usuario);

            if (usuarioLogado != null)
            {
                if (usuarioLogado.Tipo_Perfil == "Prestador")
                {
                    // ✅ Prestador só vê os próprios serviços
                    consulta = consulta.Where(s => s.UsuarioId == usuarioLogado.IdUsuario);
                }
                else if (usuarioLogado.Tipo_Perfil == "Morador" && usuarioLogado.Id_Area != null && !verTodas)
                {
                    // Morador vê serviços da mesma regional por padrão
                    consulta = consulta.Where(s =>
                        s.Usuario != null && s.Usuario.Id_Area == usuarioLogado.Id_Area
                    );
                }
                // ADMINISTRADOR vê tudo — sem filtro
                // Morador com verTodas=true também vê tudo — sem filtro
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                consulta = consulta.Where(s =>
                    s.NomeServico.Contains(searchString) ||
                    s.Profissional.Contains(searchString) ||
                    s.TipoServico.Contains(searchString) ||
                    s.Preco.Contains(searchString) ||
                    s.Contato.Contains(searchString)
                );
            }

            if (!string.IsNullOrWhiteSpace(tipo) && !tipo.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                consulta = consulta.Where(s => s.TipoServico == tipo);
            }

            int itensPorPagina = 8;
            int totalItens     = await consulta.CountAsync();
            int totalPaginas   = (int)Math.Ceiling((double)totalItens / itensPorPagina);

            if (pagina < 1) pagina = 1;

            var servicosPaginados = await consulta
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            ViewBag.PaginaAtual       = pagina;
            ViewBag.TotalPaginas      = totalPaginas;
            ViewBag.SearchString      = searchString;
            ViewBag.TipoSelecionado   = string.IsNullOrEmpty(tipo) ? "Todos" : tipo;
            ViewBag.PerfilUsuario     = usuarioLogado?.Tipo_Perfil;
            ViewBag.VerTodas          = verTodas;

            if (usuarioLogado != null)
            {
                var notificacoes = await _context.Notificacoes
                    .Where(n => n.UsuarioDestino == usuarioLogado.Nome)
                    .OrderByDescending(n => n.DataNotificacao)
                    .Take(10)
                    .ToListAsync();

                ViewBag.Notificacoes              = notificacoes;
                ViewBag.TotalNotificacoesNaoLidas = notificacoes.Count(n => !n.Lida);
            }
            else
            {
                ViewBag.Notificacoes              = new List<Notificacao>();
                ViewBag.TotalNotificacoesNaoLidas = 0;
            }

            return View(servicosPaginados);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
