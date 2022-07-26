using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nike.Benchmark.ConsumerPerformance.Models;

public class MessageAggregateRootEntityTypeConfiguration : IEntityTypeConfiguration<MessageAggregateRoot>
{
    public void Configure(EntityTypeBuilder<MessageAggregateRoot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Identifier).IsRequired();
        builder.Property(p => p.IncreamentalValue).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
        builder.Property(p => p.InstantiateAt).IsRequired();
        builder.Property(p => p.ProduceAt).IsRequired();
        builder.Property(p => p.ConsumeAt).IsRequired();
        builder.Property(p => p.StoreAt).IsRequired();

        builder.ToTable("MessageAggregateRoots");
    }
}

public class Message2AggregateRootEntityTypeConfiguration : IEntityTypeConfiguration<Message2AggregateRoot>
{
    public void Configure(EntityTypeBuilder<Message2AggregateRoot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Identifier).IsRequired();
        builder.Property(p => p.IncreamentalValue).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
        builder.Property(p => p.InstantiateAt).IsRequired();
        builder.Property(p => p.ProduceAt).IsRequired();
        builder.Property(p => p.ConsumeAt).IsRequired();
        builder.Property(p => p.StoreAt).IsRequired();

        builder.ToTable("Message2AggregateRoots");
    }
}
