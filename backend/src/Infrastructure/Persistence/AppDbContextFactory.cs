using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

/// <summary>
/// Фабрика для создания контекста базы данных во время проектирования (для миграций EF Core).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Определяем путь к проекту API, если команда запущена из папки Infrastructure или корня backend
        var currentDir = Directory.GetCurrentDirectory();
        var apiProjectPath = Directory.Exists(Path.Combine(currentDir, "src", "API"))
            ? Path.Combine(currentDir, "src", "API")
            : Path.GetFullPath(Path.Combine(currentDir, "..", "API"));

        var basePath = Directory.Exists(apiProjectPath) ? apiProjectPath : currentDir;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Строка подключения 'DefaultConnection' не найдена ни в appsettings проекта API, ни в переменных окружения."
            );
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}