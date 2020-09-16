using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nike.CustomerManagement.Domain.Customers;

namespace Nike.CustomerManagement.Infrastructure.EntityConfigurations
{
    public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.OwnsOne(p => p.FullName, navigationBuilder =>
            {
                navigationBuilder.Property(p => p.FirstName).HasColumnName("FirstName");
                navigationBuilder.Property(p => p.LastName).HasColumnName("LastName");
            });

            builder.OwnsOne(p => p.NationalCode, navigationBuilder =>
            {
                navigationBuilder.Property(p => p.Code).HasColumnName("NationalCode");
            });

            builder.ToTable("Customers");
        }
    }
}