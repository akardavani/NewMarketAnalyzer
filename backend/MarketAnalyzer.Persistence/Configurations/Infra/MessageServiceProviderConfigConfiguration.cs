using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Abstractions.Messaging;

namespace MarketAnalyzer.Persistence.Configurations.Infra
{
    internal class MessageServiceProviderConfigConfiguration : IEntityTypeConfiguration<MessageServiceProviderConfig>
    {
        public void Configure(EntityTypeBuilder<MessageServiceProviderConfig> builder)
        {
            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Config)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(500);
        }
    }
}
