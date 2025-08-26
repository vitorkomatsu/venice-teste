namespace Venice.Teste.Backend.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public double Valor { get; set; }
        public DateTime? DataValidade { get; set; }
    }
}
