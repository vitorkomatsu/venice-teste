namespace Venice.Teste.Backend.Application.DTOs.Request
{
    public class ProductRequest
    {
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public double Valor { get; set; }
        public DateTime? DataValidade { get; set; }
    }
}