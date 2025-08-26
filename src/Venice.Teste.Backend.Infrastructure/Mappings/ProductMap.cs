using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venice.Teste.Backend.Domain.Entities;

namespace Venice.Teste.Backend.Infrastructure.Mappings
{
    internal class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired();
            builder.Property(x => x.Quantidade).IsRequired();
            builder.Property(x => x.Valor);
            builder.Property(x => x.DataValidade);
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedBy);
            builder.Property(x => x.UpdatedAt);
        }
    }
}