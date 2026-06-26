using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public static readonly Guid MoutsId = Guid.Parse("9c0e7e2a-1111-4f3a-8b1a-000000000001");
    public static readonly Guid AmbevId = Guid.Parse("9c0e7e2a-1111-4f3a-8b1a-000000000002");

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("uuid");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.HasData(
            new Customer { Id = MoutsId, Name = "Mouts" },
            new Customer { Id = AmbevId, Name = "Ambev" }
        );
    }
}
