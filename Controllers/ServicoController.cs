using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;
using System.Linq;

namespace ViziLogin.Controllers
{
    public class ServicoController : Controller
    {
        private readonly AppDbContext _context;

        public ServicoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult NovoServico()
        {
            var emailLogado = User.Identity.Name;
            var usuarioNoBanco = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);

            var model = new Servico();

            if (usuarioNoBanco != null)
            {
                model.Profissional = usuarioNoBanco.Nome;
                model.UsuarioId = usuarioNoBanco.IdUsuario;

            }
            else
            {
                // Apenas para teste, caso não esteja logado de verdade:
                model.Profissional = "Usuário não identificado";
            }

            return View(model);
        }

        public async Task<IActionResult> ServicoDetalhado(int id)
        {
            var servico = await _context.Servicos.FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null) return NotFound();

            // 1. Busca as avaliações filtradas
            var avaliacoes = await _context.Avaliacao
                .Where(a => a.ServicoId == id)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            // --- NOVO BLOCO DE CÁLCULO ---
            int quantidade = avaliacoes.Count;
            // Calcula a média se houver avaliações, caso contrário define como 0
            double media = quantidade > 0 ? avaliacoes.Average(a => Convert.ToDouble(a.Nota)) : 0.0;

            // Passamos a lista, a média e o total para a ViewBag
            ViewBag.ListaAvaliacoes = avaliacoes;
            ViewBag.MediaNotas = media;
            ViewBag.TotalAvaliacoes = quantidade;
            // ----------------------------

            // Busca o usuário logado para saber o NOME dele
            var emailLogado = User.Identity.Name;
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailLogado);


            bool éDonoDoServico = usuarioLogado != null && servico.Profissional == usuarioLogado.Nome;

            bool jáAvaliou = false;
            if (usuarioLogado != null)
            {
                jáAvaliou = await _context.Avaliacao
                    .AnyAsync(a => a.ServicoId == id && a.NomeAvaliador == usuarioLogado.Nome);
            }

            ViewBag.PodeAvaliar = !éDonoDoServico && !jáAvaliou;
            ViewBag.NomeUsuarioLogado = usuarioLogado?.Nome;
            var donoDoServico = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Nome == servico.Profissional);
            ViewBag.EmailProfissional = donoDoServico?.Email;

            return View(servico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Recomendado para segurança contra ataques CSRF
        public IActionResult Editar(Servico servicoEditado)
        {
            // 1. Localiza o serviço original no banco usando o ID (que está escondido no modal)
            var servicoNoBanco = _context.Servicos.FirstOrDefault(s => s.Id == servicoEditado.Id);

            if (servicoNoBanco == null)
            {
                return NotFound();
            }

            // 2. Atualiza apenas os campos que você permitiu editar no modal
            servicoNoBanco.NomeServico = servicoEditado.NomeServico;
            servicoNoBanco.Descricao = servicoEditado.Descricao;
            servicoNoBanco.Contato = servicoEditado.Contato;
            servicoNoBanco.Preco = servicoEditado.Preco;

            // Se você tiver outros campos como 'TipoServico' e quiser editá-los, adicione aqui

            try
            {
                // 3. Salva as alterações no banco de dados
                _context.Servicos.Update(servicoNoBanco);
                _context.SaveChanges();

                TempData["Sucesso"] = "Serviço atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao atualizar o serviço: " + ex.Message;
            }

            // 4. Redireciona de volta para a mesma página de detalhes para ver as mudanças
            return RedirectToAction("ServicoDetalhado", new { id = servicoEditado.Id });
        }

        [HttpPost]
        [Route("Servicos/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // 1. Busca o serviço no banco de dados pelo ID enviado pelo formulário
            var servico = _context.Servicos.FirstOrDefault(s => s.Id == id);

            // Se o serviço não existir, retorna erro 404
            if (servico == null)
            {
                return NotFound();
            }

            // 2. Busca as informações do usuário que está logado no momento
            var emailLogado = User.Identity.Name;
            var usuarioLogado = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);

            // 3. VALIDAÇÃO DE SEGURANÇA: 
            // Só permite deletar se o usuário logado for o mesmo que criou o serviço (comparando os nomes)
            if (usuarioLogado == null || servico.Profissional != usuarioLogado.Nome)
            {
                TempData["Erro"] = "Acesso negado! Você só pode excluir os seus próprios serviços.";
                return RedirectToAction("ServicoDetalhado", new { id = id });
            }

            try
            {
                // 4. Se passou na validação, remove do banco e salva as alterações
                _context.Servicos.Remove(servico);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro técnico no banco de dados
                TempData["Erro"] = "Ocorreu um erro ao tentar excluir o serviço.";
                return RedirectToAction("ServicoDetalhado", new { id = id });
            }

            // 5. Redireciona para a página inicial (Action Index do HomeController)
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Criar(Servico servico)
        {
            // Força o preenchimento do profissional novamente por segurança
            var emailLogado = User.Identity.Name;
            var usuarioNoBanco = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);

            if (usuarioNoBanco != null)
            {
                servico.Profissional = usuarioNoBanco.Nome;
            }

            // Remove a validação para que o ModelState não reclame do campo readonly
            ModelState.Remove("Profissional");

            if (ModelState.IsValid)
            {
                _context.Servicos.Add(servico);
                _context.SaveChanges(); // Aqui ele grava na tabela dbo.Servicos
                return RedirectToAction("Index", "Home");
            }

            return View("NovoServico", servico);
        }
    }
}

