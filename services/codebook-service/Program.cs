using CodebookService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// Baza podataka — EF Core + PostgreSQL
// ---------------------------------------------------------------
builder.Services.AddDbContext<CodebookDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "hr_codebook")
    )
);

// ---------------------------------------------------------------
// Swagger
// ---------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR-lite Codebook Service", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CodebookService.xml"));
});

builder.Services.AddControllers();

var app = builder.Build();

// ---------------------------------------------------------------
// Automatska primjena migracija pri pokretanju
//
// EF Core provjerava tablicu __ef_migrations_history i primjenjuje
// sve migracije koje još nisu aplicirane. Ovo je ekvivalent ručnog
// poziva: dotnet ef database update
// ---------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CodebookDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
