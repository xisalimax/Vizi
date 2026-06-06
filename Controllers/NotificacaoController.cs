using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;

namespace ViziLogin.Controllers
{
    public class NotificacaoController : Controller
    {
        private readonly AppDbContext _context;

        public NotificacaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLidas()
        {
            var emailLogado = User.Identity.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == emailLogado);

            if (usuario != null)
            {
                var notificacoes = await _context.Notificacoes
                    .Where(n =>
                        n.UsuarioDestino == usuario.Nome &&
                        n.Lida == false)
                    .ToListAsync();

                foreach (var notificacao in notificacoes)
                {
                    notificacao.Lida = true;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}