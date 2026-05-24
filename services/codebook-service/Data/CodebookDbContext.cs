using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Data;

public class CodebookDbContext : DbContext
{
    public CodebookDbContext(DbContextOptions<CodebookDbContext> options) : base(options) { }

    // ---------------------------------------------------------------
    // DbSet-ovi — jedna property po šifarniku
    // ---------------------------------------------------------------
    public DbSet<Gender>       Genders       { get; set; }
    public DbSet<Country>      Countries     { get; set; }
    public DbSet<County>       Counties      { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Settlement>   Settlements   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sve tablice idu u hr_codebook shemu
        modelBuilder.HasDefaultSchema("hr_codebook");

        ConfigureGender(modelBuilder);
        ConfigureCountry(modelBuilder);
        ConfigureCounty(modelBuilder);
        ConfigureMunicipality(modelBuilder);
        ConfigureSettlement(modelBuilder);
    }

    // ---------------------------------------------------------------
    // SaveChanges override — automatsko postavljanje audit kolona
    // Pokriva sve entitete koji nasljeđuju CodebookBase.
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

        foreach (var entry in ChangeTracker.Entries<CodebookBase>())
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
                // UpdatedBy postavljaju servisni sloj iz JWT claims.
            }
        }
    }

    // ---------------------------------------------------------------
    // Konfiguracija entiteta
    // ---------------------------------------------------------------

    private static void ConfigureGender(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("codebook_gender");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NameEn).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);

            entity.HasIndex(e => e.Code).IsUnique();

            // -------------------------------------------------------
            // SEED DATA — kako funkcionira HasData() u EF Core
            // -------------------------------------------------------
            //
            // HasData() govori EF Core-u da ovi zapisi trebaju postojati
            // u bazi. EF Core ih uključuje u generiranu migraciju kao
            // migrationBuilder.InsertData() pozive.
            //
            // PRAVILA za HasData():
            //  1. ID mora biti HARDKODIRAN Guid — nikad Guid.NewGuid().
            //     Razlog: migracija se generira jednom i mora biti ista
            //     pri svakom pokretanju (idempotentnost).
            //
            //  2. Ne mogu se koristiti navigation properties (npr. lista
            //     objekata). Samo skalarni FK-ovi (Guid, string, int).
            //
            //  3. Ako se seed zapis promijeni, EF Core generira UPDATE
            //     u novoj migraciji — ne briše i ne duplicira.
            //
            //  4. CreatedBy = Guid.Empty → konvencija: sistem je kreirao
            //     zapis, ne korisnik. (Definirano u CLAUDE.md)
            //
            entity.HasData(
                new Gender
                {
                    Id        = new Guid("a1000000-0000-0000-0000-000000000001"),
                    Code      = "M",
                    Name      = "Muško",
                    NameEn    = "Male",
                    IsActive  = true,
                    Ordinal   = 1,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = Guid.Empty,   // sistem
                    UpdatedBy = Guid.Empty
                },
                new Gender
                {
                    Id        = new Guid("a1000000-0000-0000-0000-000000000002"),
                    Code      = "F",
                    Name      = "Žensko",
                    NameEn    = "Female",
                    IsActive  = true,
                    Ordinal   = 2,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = Guid.Empty,
                    UpdatedBy = Guid.Empty
                },
                new Gender
                {
                    Id        = new Guid("a1000000-0000-0000-0000-000000000003"),
                    Code      = "O",
                    Name      = "Ostalo",
                    NameEn    = "Other",
                    IsActive  = true,
                    Ordinal   = 3,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = Guid.Empty,
                    UpdatedBy = Guid.Empty
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
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameEn).HasMaxLength(200);
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
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameEn).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            // CountyId je logički FK — bez DB constraint prema codebook_country
        });
    }

    private static void ConfigureMunicipality(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.ToTable("codebook_municipality");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameEn).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
        });
    }

    private static void ConfigureSettlement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("codebook_settlement");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameEn).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
        });
    }
}
