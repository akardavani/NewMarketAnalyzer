using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class ContactUsConfiguration : IEntityTypeConfiguration<ContactUs>
    {
        public void Configure(EntityTypeBuilder<ContactUs> builder)
        {
            builder.Property(e => e.Subject)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(e => e.Text)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(11);

            builder.Property(e => e.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(200);

            builder.Property(e => e.IpAddress)
                .HasMaxLength(15)
                .IsRequired();
        }
    }
}
