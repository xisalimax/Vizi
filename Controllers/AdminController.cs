using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;

namespace ViziLogin.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Totais
            ViewBag.TotalUsuarios   = await _context.Usuarios.CountAsync();
            ViewBag.TotalServicos   = await _context.Servicos.CountAsync();
            ViewBag.TotalAvaliacoes = await _context.Avaliacao.CountAsync();
            ViewBag.TotalDenuncias  = await _context.Denuncias.CountAsync();

            // Listas
            ViewBag.Usuarios  = await _context.Usuarios.OrderByDescending(u => u.Inclusao).ToListAsync();
            ViewBag.Servicos  = await _context.Servicos.Include(s => s.Usuario).OrderByDescending(s => s.Id).ToListAsync();
            ViewBag.Denuncias = await _context.Denuncias.Include(d => d.Servico).OrderByDescending(d => d.DataDenuncia).ToListAsync();

            // Relatório — usuários por perfil
            ViewBag.TotalMoradores   = await _context.Usuarios.CountAsync(u => u.Tipo_Perfil == "Morador");
            ViewBag.TotalPrestadores = await _context.Usuarios.CountAsync(u => u.Tipo_Perfil == "Prestador");
            ViewBag.TotalAdmins      = await _context.Usuarios.CountAsync(u => u.Tipo_Perfil == "ADMINISTRADOR");

            // Relatório — serviços por tipo
            ViewBag.ServicoPorTipo = await _context.Servicos
                .GroupBy(s => s.TipoServico)
                .Select(g => new { Tipo = g.Key, Total = g.Count() })
                .ToListAsync();

            // Relatório — média geral (fix CS0173: cast para double)
            ViewBag.MediaGeralAvaliacoes = await _context.Avaliacao.AnyAsync()
                ? (double)await _context.Avaliacao.AverageAsync(a => a.Nota)
                : 0.0;

            // Relatório — top 5 serviços mais avaliados
            ViewBag.TopServicos = await _context.Avaliacao
                .GroupBy(a => a.ServicoId)
                .Select(g => new
                {
                    ServicoId = g.Key,
                    Total     = g.Count(),
                    Media     = g.Average(a => a.Nota)
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            var topIds = ((IEnumerable<dynamic>)ViewBag.TopServicos)
                .Select(x => (int)x.ServicoId).ToList();

            ViewBag.TopServicosNomes = await _context.Servicos
                .Where(s => topIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.NomeServico);

            // Relatório — últimos 6 meses
            var seisAtras = DateTime.Now.AddMonths(-6);

            ViewBag.DenunciasPorMes = await _context.Denuncias
                .Where(d => d.DataDenuncia >= seisAtras)
                .GroupBy(d => new { d.DataDenuncia.Year, d.DataDenuncia.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            ViewBag.CadastrosPorMes = await _context.Usuarios
                .Where(u => u.Inclusao.HasValue && u.Inclusao.Value >= seisAtras)
                .GroupBy(u => new { u.Inclusao!.Value.Year, u.Inclusao.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return View();
        }

        // Confirmar denúncia — remove serviço e notifica prestador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarDenuncia(int id)
        {
            var denuncia = await _context.Denuncias
                .Include(d => d.Servico)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (denuncia == null)
            {
                TempData["Sucesso"] = "Denúncia não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var servico = denuncia.Servico;

            if (servico != null)
            {
                var notificacao = new Notificacao
                {
                    UsuarioDestino  = servico.Profissional,
                    Mensagem        = $"⚠️ Seu serviço \"{servico.NomeServico}\" foi removido após análise de uma denúncia pelo administrador.",
                    DataNotificacao = DateTime.Now,
                    Lida            = false,
                    ServicoId       = servico.Id
                };

                _context.Notificacoes.Add(notificacao);
                _context.Servicos.Remove(servico);
            }

            _context.Denuncias.Remove(denuncia);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Denúncia confirmada. Serviço \"{servico?.NomeServico}\" removido e prestador notificado.";
            return RedirectToAction(nameof(Index));
        }

        // Denúncia improcedente — só arquiva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolverDenuncia(int id)
        {
            var denuncia = await _context.Denuncias.FindAsync(id);
            if (denuncia != null)
            {
                _context.Denuncias.Remove(denuncia);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Denúncia marcada como improcedente e arquivada.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Remover usuário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = $"Usuário '{usuario.Nome}' removido com sucesso.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Remover serviço
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverServico(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = $"Serviço '{servico.NomeServico}' removido com sucesso.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
