using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Data;

public class CodebookDbContext : DbContext
{
    public CodebookDbContext(DbContextOptions<CodebookDbContext> options) : base(options) { }

    // ---------------------------------------------------------------
    // DbSet-ovi
    // ---------------------------------------------------------------
    public DbSet<Language>     Languages      { get; set; }
    public DbSet<Translation>  Translations   { get; set; }
    public DbSet<Gender>       Genders        { get; set; }
    public DbSet<Country>      Countries      { get; set; }
    public DbSet<County>       Counties       { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Settlement>   Settlements    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("hr_codebook");

        ConfigureLanguage(modelBuilder);
        ConfigureTranslation(modelBuilder);
        ConfigureGender(modelBuilder);
        ConfigureCountry(modelBuilder);
        ConfigureCounty(modelBuilder);
        ConfigureMunicipality(modelBuilder);
        ConfigureSettlement(modelBuilder);
    }

    // ---------------------------------------------------------------
    // SaveChanges override — automatsko postavljanje audit kolona
    // Pokriva sve entitete koji implementiraju IAuditable:
    //   CodebookBase (i sve nasljednice), Language, Translation.
    // ---------------------------------------------------------------
    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                // CreatedBy i UpdatedBy postavljaju se u servisnom sloju
                // iz JWT claims prije poziva SaveChanges().
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                // UpdatedBy postavlja servisni sloj iz JWT claims.
            }
        }
    }

    // ---------------------------------------------------------------
    // Seed konstante
    // ---------------------------------------------------------------
    // Language codes (PK) — string umjesto Guid
    internal const string LangHr = "hr";
    internal const string LangEn = "en";
    // Gender
    internal static readonly Guid GenderMId = new("a1000000-0000-0000-0000-000000000001");
    internal static readonly Guid GenderFId = new("a1000000-0000-0000-0000-000000000002");
    internal static readonly Guid GenderOId = new("a1000000-0000-0000-0000-000000000003");
    // Translations (Gender)
    internal static readonly Guid TrMHrId = new("c0000000-0000-0000-0000-000000000001");
    internal static readonly Guid TrMEnId = new("c0000000-0000-0000-0000-000000000002");
    internal static readonly Guid TrFHrId = new("c0000000-0000-0000-0000-000000000003");
    internal static readonly Guid TrFEnId = new("c0000000-0000-0000-0000-000000000004");
    internal static readonly Guid TrOHrId = new("c0000000-0000-0000-0000-000000000005");
    internal static readonly Guid TrOEnId = new("c0000000-0000-0000-0000-000000000006");

    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // ---------------------------------------------------------------
    // Konfiguracija entiteta
    // ---------------------------------------------------------------

    private static void ConfigureLanguage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("language");
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            // ----------------------------------------------------------
            // SEED: podržani jezici
            //
            // Code je PK — čitljiv, jedinstven, bez JOIN-a.
            // Novi jezik = novi redak ovdje + nova HasData za prijevode.
            // ----------------------------------------------------------
            entity.HasData(
                new Language
                {
                    Code      = LangHr,
                    Name      = "Hrvatski",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Language
                {
                    Code      = LangEn,
                    Name      = "English",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                }
            );
        });
    }

    private static void ConfigureTranslation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Translation>(entity =>
        {
            entity.ToTable("translation");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LanguageCode).HasMaxLength(10).IsRequired();
            entity.Property(e => e.FieldName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();

            // Unique constraint: jedan prijevod po (entitet, zapis, jezik, polje)
            entity.HasIndex(e => new { e.EntityType, e.EntityId, e.LanguageCode, e.FieldName })
                  .IsUnique();

            // FK prema language.Code (ista shema — dozvoljeno)
            entity.HasOne<Language>()
                  .WithMany()
                  .HasForeignKey(e => e.LanguageCode)
                  .OnDelete(DeleteBehavior.Restrict);

            // ----------------------------------------------------------
            // SEED: prijevodi za Gender šifarnik
            //
            // PRAVILA za HasData():
            //  1. Svi ID-evi moraju biti HARDKODIRANI — nikad Guid.NewGuid().
            //  2. Ne koristiti navigation properties — samo skalarni FK-ovi.
            //  3. Hrvatski ("hr") prijevod je OBAVEZAN za sve šifarnike.
            // ----------------------------------------------------------
            entity.HasData(
                // M — Muško / Male
                new Translation
                {
                    Id = TrMHrId, EntityType = "codebook_gender", EntityId = GenderMId,
                    LanguageCode = LangHr, FieldName = "Name", Value = "Muško",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Translation
                {
                    Id = TrMEnId, EntityType = "codebook_gender", EntityId = GenderMId,
                    LanguageCode = LangEn, FieldName = "Name", Value = "Male",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                // F — Žensko / Female
                new Translation
                {
                    Id = TrFHrId, EntityType = "codebook_gender", EntityId = GenderFId,
                    LanguageCode = LangHr, FieldName = "Name", Value = "Žensko",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Translation
                {
                    Id = TrFEnId, EntityType = "codebook_gender", EntityId = GenderFId,
                    LanguageCode = LangEn, FieldName = "Name", Value = "Female",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                // O — Ostalo / Other
                new Translation
                {
                    Id = TrOHrId, EntityType = "codebook_gender", EntityId = GenderOId,
                    LanguageCode = LangHr, FieldName = "Name", Value = "Ostalo",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Translation
                {
                    Id = TrOEnId, EntityType = "codebook_gender", EntityId = GenderOId,
                    LanguageCode = LangEn, FieldName = "Name", Value = "Other",
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                }
            );
        });
    }

    private static void ConfigureGender(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("codebook_gender");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.HasIndex(e => e.Code).IsUnique();

            // Seed: samo Code/IsActive/Ordinal — nazivi su u Translation tablici
            entity.HasData(
                new Gender
                {
                    Id = GenderMId, Code = "M", IsActive = true, Ordinal = 1,
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Gender
                {
                    Id = GenderFId, Code = "F", IsActive = true, Ordinal = 2,
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                },
                new Gender
                {
                    Id = GenderOId, Code = "O", IsActive = true, Ordinal = 3,
                    CreatedAt = SeedDate, UpdatedAt = SeedDate,
                    CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty
                }
            );
        });
    }

    private static void ConfigureCountry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("codebook_country");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }

    private static void ConfigureCounty(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<County>(entity =>
        {
            entity.ToTable("codebook_county");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.CountryId).IsRequired(false);
        });
    }

    private static void ConfigureMunicipality(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.ToTable("codebook_municipality");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.CountyId).IsRequired(false);
            entity.Property(e => e.JOPPDCode).HasMaxLength(20).IsRequired(false);
        });
    }

    private static void ConfigureSettlement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("codebook_settlement");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.MunicipalityId).IsRequired(false);
        });
    }
}
