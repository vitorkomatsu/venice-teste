using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venice.Teste.Backend.Domain.Entities;

namespace Venice.Teste.Backend.Infrastructure.Mappings
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClienteId).IsRequired();
            builder.Property(x => x.Data).HasColumnType("datetimeoffset").IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Total).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}