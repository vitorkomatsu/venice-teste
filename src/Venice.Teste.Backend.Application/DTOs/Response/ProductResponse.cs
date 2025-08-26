namespace Venice.Teste.Backend.Application.DTOs.Response
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public double Valor { get; set; }
        public DateTime? DataValidade { get; set; }
    }
}