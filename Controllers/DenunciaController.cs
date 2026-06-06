using Microsoft.AspNetCore.Mvc;
using ViziLogin.Data;
using ViziLogin.Models;

namespace ViziLogin.Controllers
{
    public class DenunciaController : Controller
    {
        private readonly AppDbContext _context;

        public DenunciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(int ServicoId, string Motivo, string NomeUsuario)
        {
            var denuncia = new Denuncia
            {
                ServicoId = ServicoId,
                Motivo = Motivo,
                NomeUsuario = NomeUsuario,
                DataDenuncia = DateTime.Now
            };

            _context.Denuncias.Add(denuncia);
            _context.SaveChanges();

            TempData["Sucesso"] = "Denúncia enviada com sucesso!";

            return RedirectToAction(
                "ServicoDetalhado",
                "Servico",
                new { id = ServicoId }
            );
        }
    }
}