using CodebookService.Data;
using CodebookService.Repositories;
using CodebookService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Baza podataka ────────────────────────────────────────────────────
builder.Services.AddDbContext<CodebookDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "hr_codebook")
    )
);

// ── Repository i Service registracija ───────────────────────────────
builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IGenderService,    GenderService>();

// ── CORS — dozvoli frontend (dev i produkcija) ───────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── Swagger ──────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR-lite Codebook Service", Version = "v1" });
});

builder.Services.AddControllers();

var app = builder.Build();

// ── Automatska primjena migracija pri pokretanju ─────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CodebookDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
