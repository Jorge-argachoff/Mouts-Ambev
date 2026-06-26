using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public static readonly Guid CervejaId = Guid.Parse("9c0e7e2a-2222-4f3a-8b1a-000000000001");
    public static readonly Guid AguaId = Guid.Parse("9c0e7e2a-2222-4f3a-8b1a-000000000002");

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("uuid");

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasData(
            new Product { Id = CervejaId, Name = "Cerveja", UnitPrice = 8.50m },
            new Product { Id = AguaId, Name = "Água", UnitPrice = 3.00m }
        );
    }
}
