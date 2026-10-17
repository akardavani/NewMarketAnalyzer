using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Persistence.Configurations.Infra
{
    internal class OutgoingMessageConfiguration : IEntityTypeConfiguration<OutgoingMessage>
    {
        public void Configure(EntityTypeBuilder<OutgoingMessage> builder)
        {
            builder.Property(e => e.Subject)
                .HasMaxLength(500);

            builder.Property(e => e.MessageBody)
                .IsRequired();

            builder.Property(e => e.Recipient)
                .IsRequired();
        }
    }
}
