using System.Text.Json.Serialization;

namespace Venice.Teste.Backend.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoCusto
    {
        Insumo,
        MaoDeObra,
        ServicoTerceirizado,
        Melhorias,
        CustoFixo, // Ex: energia, aluguel
        CustoVariavel, // Ex: transporte, embalagem
        Administrativo
    }
}
