namespace ViziLogin.Models
{
    public class PrestadorPerfilViewModel
    {
        public Usuario ? Usuario { get; set; }

        public List<Servico> ? Servicos { get; set; }

        public List<Avaliacao> ? Avaliacoes { get; set; }

        public decimal NotaMedia { get; set; }

        public int QuantidadeAvaliacoes { get; set; }
    }
}