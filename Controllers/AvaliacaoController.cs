using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;

namespace ViziLogin.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly AppDbContext _context;

        public AvaliacaoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var avaliacoes = await _context.Avaliacao
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            return View(avaliacoes);
        }



        [HttpPost]
        public async Task<IActionResult> SalvarAvaliacao(Avaliacao avaliacao)
        {
            var emailLogado = User.Identity.Name;
            var servico = await _context.Servicos.FindAsync(avaliacao.ServicoId);

            // Verifica novamente se é o dono tentando burlar o sistema
            if (servico.Profissional == emailLogado)
            {
                return BadRequest("Você não pode avaliar seu próprio serviço.");
            }

            // Verifica se já existe avaliação desse usuário para esse serviço
            var existente = await _context.Avaliacao
                .AnyAsync(a => a.ServicoId == avaliacao.ServicoId && a.NomeAvaliador == emailLogado);

            if (existente)
            {
                return BadRequest("Você já avaliou este serviço.");
            }

            // Se passou por tudo, salva
            _context.Avaliacao.Add(avaliacao);
            await _context.SaveChangesAsync();

            return RedirectToAction("ServicoDetalhado", new { id = avaliacao.ServicoId });
        }



        [HttpPost]
        public async Task<IActionResult> Create([Bind("NomeAvaliador,Nota,Comentario,ServicoId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                avaliacao.DataAvaliacao = DateTime.Now;

                _context.Avaliacao.Add(avaliacao);
                await _context.SaveChangesAsync();

                var servico = await _context.Servicos
                    .FirstOrDefaultAsync(s => s.Id == avaliacao.ServicoId);

                if (servico != null)
                {
                    var notificacao = new Notificacao
                    {
                        UsuarioDestino = servico.Profissional,
                        ServicoId = servico.Id,
                        Mensagem = $"{avaliacao.NomeAvaliador} avaliou seu serviço \"{servico.NomeServico}\" com nota {avaliacao.Nota}.",
                        DataNotificacao = DateTime.Now,
                        Lida = false
                    };

                    _context.Notificacoes.Add(notificacao);
                    await _context.SaveChangesAsync();
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }

            return BadRequest("Dados inválidos");
        }
    }
}