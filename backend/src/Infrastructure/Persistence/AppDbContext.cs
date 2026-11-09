using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public const string CaseInsensetiveCollation = "case_insensitive";

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Регистранезависимая сортировка, котороая будет использоваться для сравнения email пользователей
        modelBuilder.HasCollation(name: "case_insensitive", locale: "en-u-ks-primary", provider: "icu", deterministic: false);

        // Глобально для всех сущностей преобразуем DateTimeOffset в UTC при отправке в БД
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?));

            foreach (var property in properties)
            {
                property.SetValueConverter(new ValueConverter<DateTimeOffset, DateTimeOffset>(
                    v => v.ToUniversalTime(),
                    v => v.ToUniversalTime()));
            }
        }
    }
}