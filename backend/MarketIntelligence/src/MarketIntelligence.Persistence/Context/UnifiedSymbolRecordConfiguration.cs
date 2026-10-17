
using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class UnifiedSymbolRecordConfiguration : IEntityTypeConfiguration<UnifiedSymbolRecord>
    {
        public void Configure(EntityTypeBuilder<UnifiedSymbolRecord> entity)
        {
            entity.ToTable("UnifiedSymbols");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.MarketId)
                .IsRequired();

            entity.Property(x => x.SymbolKey)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Symbol)
                .HasMaxLength(100);

            entity.Property(x => x.InstrumentId)
                .HasMaxLength(100);

            entity.Property(x => x.InsCode)
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .HasMaxLength(300);

            entity.Property(x => x.PersianName)
                .HasMaxLength(300);

            entity.Property(x => x.ParentSymbol)
                .HasMaxLength(100);

            entity.Property(x => x.MetadataJson);

            entity.HasIndex(x => new { x.MarketId, x.SymbolKey })
                .IsUnique();

            entity.HasIndex(x => x.Symbol);

            entity.HasIndex(x => x.InstrumentId);

            entity.HasIndex(x => x.InsCode);

            entity.HasOne(x => x.Market)
                .WithMany()
                .HasForeignKey(x => x.MarketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}



//using MarketIntelligence.Domain;
//using Microsoft.EntityFrameworkCore;

//namespace MarketIntelligence.Infrastructure
//{
//    public class MarketDbContext : DbContext
//    {
//        public DbSet<UnifiedCandleRecord> UnifiedCandles { get; set; }
//        public DbSet<UnifiedSymbolRecord> UnifiedSymbols { get; set; }
//        public DbSet<MarketRecord> Markets => Set<MarketRecord>();


//        public MarketDbContext(DbContextOptions<MarketDbContext> options)
//            : base(options)
//        {
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // ثبت خودکار همه Entity ها
//            var entityTypes = typeof(MacroCandle).Assembly
//                .GetTypes()
//                .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

//            foreach (var type in entityTypes)
//                modelBuilder.Entity(type);

//            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketDbContext).Assembly);


//            modelBuilder.Entity<UnifiedCandleRecord>(e =>
//            {
//                e.HasIndex(x => new { x.MarketId, x.Symbol, x.Timeframe, x.Timestamp });
//            });

//            modelBuilder.Entity<UnifiedSymbolRecord>(e =>
//            {
//                e.HasIndex(x => new { x.MarketId, x.SymbolKey }).IsUnique();

//                e.HasIndex(x => x.Symbol);
//                e.HasIndex(x => x.InstrumentId);
//                e.HasIndex(x => x.InsCode);
//            });

//            modelBuilder.Entity<MarketRecord>(entity =>
//            {
//                entity.ToTable("Markets");

//                entity.HasKey(x => x.Id);

//                entity.Property(x => x.Code)
//                    .IsRequired()
//                    .HasMaxLength(50);

//                entity.Property(x => x.Name)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.HasIndex(x => x.Code)
//                    .IsUnique();
//            });

//            modelBuilder.Entity<UnifiedSymbolRecord>(entity =>
//            {
//                entity.ToTable("UnifiedSymbols");

//                entity.HasKey(x => x.Id);

//                entity.Property(x => x.SymbolKey)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(x => x.Symbol)
//                    .HasMaxLength(100);

//                entity.Property(x => x.InstrumentId)
//                    .HasMaxLength(100);

//                entity.Property(x => x.InsCode)
//                    .HasMaxLength(100);

//                entity.Property(x => x.Name)
//                    .HasMaxLength(300);

//                entity.Property(x => x.PersianName)
//                    .HasMaxLength(300);

//                entity.Property(x => x.ParentSymbol)
//                    .HasMaxLength(100);

//                entity.HasIndex(x => new { x.MarketId, x.SymbolKey })
//                    .IsUnique();

//                entity.HasOne(x => x.Market)
//                    .WithMany()
//                    .HasForeignKey(x => x.MarketId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });


//            modelBuilder.Entity<MarketRecord>().HasData(
//    new MarketRecord
//    {
//        Id = MarketType.Crypto,
//        Code = "Crypto",
//        Name = "رمزارز",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.Ime,
//        Code = "Ime",
//        Name = "بورس کالا",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.Macro,
//        Code = "Macro",
//        Name = "اقتصاد کلان",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.Tse,
//        Code = "Tse",
//        Name = "بورس تهران",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.InvestmentFund,
//        Code = "InvestmentFund",
//        Name = "صندوق سرمایه‌گذاری",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.RealEstate,
//        Code = "RealEstate",
//        Name = "مسکن",
//        IsActive = true
//    },
//    new MarketRecord
//    {
//        Id = MarketType.TseIndex,
//        Code = "TseIndex",
//        Name = "شاخص‌های بورس",
//        IsActive = true
//    }
//);


//            base.OnModelCreating(modelBuilder);
//        }
//    }
//}
