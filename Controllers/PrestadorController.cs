using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;
using System.Security.Claims;

namespace ViziLogin.Controllers
{
    public class PrestadorController : Controller
    {
        private readonly AppDbContext _context;

        public PrestadorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Perfil(int id)
        {
            // 1. Usuário
            var usuario = await _context.Usuarios
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            var idUsuarioLogado = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.EhDonoDoPerfil = !string.IsNullOrEmpty(idUsuarioLogado) && idUsuarioLogado == id.ToString();

            // 2. Serviços (só relevante para Prestador)
            var servicos = await _context.Servicos
                .Where(s => s.UsuarioId == id)
                .ToListAsync();

            // 3. Avaliações (só relevante para Prestador)
            var avaliacoes = await _context.Avaliacao
                .Where(a => servicos.Select(s => s.Id).Contains(a.ServicoId))
                .ToListAsync();

            // 4. Média
            decimal media = avaliacoes.Any()
                ? avaliacoes.Average(a => Convert.ToDecimal(a.Nota))
                : 0;

            // 5. ViewModel
            var model = new PrestadorPerfilViewModel
            {
                Usuario          = usuario,
                Servicos         = servicos,
                Avaliacoes       = avaliacoes,
                NotaMedia        = media,
                QuantidadeAvaliacoes = avaliacoes.Count
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(int IdUsuario, string Sobre, string TelefonePessoal, string TelefoneServico)
        {
            var usuarioNoBanco = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == IdUsuario);

            if (usuarioNoBanco == null)
                return NotFound();

            usuarioNoBanco.Sobre            = Sobre;
            usuarioNoBanco.TelefonePessoal  = TelefonePessoal;
            usuarioNoBanco.TelefoneServico  = TelefoneServico;

            await _context.SaveChangesAsync();

            return RedirectToAction("Perfil", new { id = IdUsuario });
        }
    }
}
