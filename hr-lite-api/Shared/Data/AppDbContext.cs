using HrLite.Modules.Codebook.Models;
using HrLite.Modules.Identity.Models;
using HrLite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HrLite.Shared.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Codebook ────────────────────────────────────────────────────────
    public DbSet<Language>     Languages      { get; set; }
    public DbSet<Translation>  Translations   { get; set; }
    public DbSet<Gender>       Genders        { get; set; }
    public DbSet<Country>      Countries      { get; set; }
    public DbSet<County>       Counties       { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Settlement>   Settlements    { get; set; }

    // ── Identity ─────────────────────────────────────────────────────────
    public DbSet<User>     Users     { get; set; }
    public DbSet<Role>     Roles     { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureCodebook(modelBuilder);
        ConfigureIdentity(modelBuilder);
    }

    // ── SaveChanges — automatsko postavljanje audit kolona ───────────────
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
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    // ── Seed konstante ───────────────────────────────────────────────────
    private const string LangHr = "hr";
    private const string LangEn = "en";

    private static readonly Guid GenderMId = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid GenderFId = new("a1000000-0000-0000-0000-000000000002");
    private static readonly Guid GenderOId = new("a1000000-0000-0000-0000-000000000003");

    private static readonly Guid TrMHrId = new("c0000000-0000-0000-0000-000000000001");
    private static readonly Guid TrMEnId = new("c0000000-0000-0000-0000-000000000002");
    private static readonly Guid TrFHrId = new("c0000000-0000-0000-0000-000000000003");
    private static readonly Guid TrFEnId = new("c0000000-0000-0000-0000-000000000004");
    private static readonly Guid TrOHrId = new("c0000000-0000-0000-0000-000000000005");
    private static readonly Guid TrOEnId = new("c0000000-0000-0000-0000-000000000006");

    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // ── Codebook konfiguracija ───────────────────────────────────────────
    private static void ConfigureCodebook(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("language", "hr_codebook");
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            entity.HasData(
                new Language { Code = LangHr, Name = "Hrvatski", CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Language { Code = LangEn, Name = "English",  CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty }
            );
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.ToTable("translation", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LanguageCode).HasMaxLength(10).IsRequired();
            entity.Property(e => e.FieldName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();

            entity.HasIndex(e => new { e.EntityType, e.EntityId, e.LanguageCode, e.FieldName }).IsUnique();

            entity.HasOne<Language>()
                  .WithMany()
                  .HasForeignKey(e => e.LanguageCode)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(
                new Translation { Id = TrMHrId, EntityType = "codebook_gender", EntityId = GenderMId, LanguageCode = LangHr, FieldName = "Name", Value = "Muško",   CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Translation { Id = TrMEnId, EntityType = "codebook_gender", EntityId = GenderMId, LanguageCode = LangEn, FieldName = "Name", Value = "Male",    CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Translation { Id = TrFHrId, EntityType = "codebook_gender", EntityId = GenderFId, LanguageCode = LangHr, FieldName = "Name", Value = "Žensko",  CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Translation { Id = TrFEnId, EntityType = "codebook_gender", EntityId = GenderFId, LanguageCode = LangEn, FieldName = "Name", Value = "Female",  CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Translation { Id = TrOHrId, EntityType = "codebook_gender", EntityId = GenderOId, LanguageCode = LangHr, FieldName = "Name", Value = "Ostalo",  CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Translation { Id = TrOEnId, EntityType = "codebook_gender", EntityId = GenderOId, LanguageCode = LangEn, FieldName = "Name", Value = "Other",   CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty }
            );
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("codebook_gender", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasData(
                new Gender { Id = GenderMId, Code = "M", IsActive = true, Ordinal = 1, CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Gender { Id = GenderFId, Code = "F", IsActive = true, Ordinal = 2, CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty },
                new Gender { Id = GenderOId, Code = "O", IsActive = true, Ordinal = 3, CreatedAt = SeedDate, UpdatedAt = SeedDate, CreatedBy = Guid.Empty, UpdatedBy = Guid.Empty }
            );
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("codebook_country", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.ToTable("codebook_county", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.CountryId).IsRequired(false);
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.ToTable("codebook_municipality", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.CountyId).IsRequired(false);
            entity.Property(e => e.JOPPDCode).HasMaxLength(20).IsRequired(false);
        });

        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("codebook_settlement", "hr_codebook");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Ordinal).HasDefaultValue(0);
            entity.Property(e => e.MunicipalityId).IsRequired(false);
            entity.Property(e => e.PostalNumber).HasMaxLength(20).IsRequired(false);
        });
    }

    // ── Identity konfiguracija ───────────────────────────────────────────
    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "hr_identity");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles", "hr_identity");
            entity.HasKey(e => e.Id);

            entity.HasData(
                new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Admin" },
                new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "HR" },
                new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Manager" },
                new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Employee" }
            );
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles", "hr_identity");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
        });
    }
}
