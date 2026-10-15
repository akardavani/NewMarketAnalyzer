using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Abstractions.ScheduledTasks;

namespace MarketAnalyzer.Persistence.Configurations.Infra
{
    internal class ScheduledTaskConfiguration : IEntityTypeConfiguration<ScheduledTask>
    {
        public void Configure(EntityTypeBuilder<ScheduledTask> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Description)
                .HasMaxLength(1000);
        }
    }
}
