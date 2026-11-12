using Domain.Exceptions.Projects;
using Domain.Exceptions.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Infrastructure.Persistence;

public sealed class ExceptionTranslationInterceptor : SaveChangesInterceptor
{
    private static readonly Dictionary<string, Func<Exception>> ExceptionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ix_users_email"] = () => new UserAlreadyExistsException(),
    };

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Exception is DbUpdateException { InnerException: PostgresException pgEx })
            throw Translate(pgEx, eventData.Exception);

        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        return eventData.Exception is DbUpdateException { InnerException: PostgresException pgEx }
            ? throw Translate(pgEx, eventData.Exception)
            : base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static Exception Translate(PostgresException pgEx, Exception fallbackException)
    {
        // Базовая проверка, что это именно те типы ошибок БД, которые мы вообще хотим перехватывать
        var isConstraintViolation = pgEx.SqlState is PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ExclusionViolation;

        // Обрабатывает случай, если проект успел удалиться за доли секунды до попытки создать в нем задачу и получился foreign_key_violation exception
        if (pgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            if (pgEx.ConstraintName?.Contains("project_id") == true)
                throw new ProjectNotFoundException(Guid.Empty);

            if (pgEx.ConstraintName?.Contains("assignee_id") == true)
                throw new UserNotFoundException(Guid.Empty);
        }

        if (isConstraintViolation &&
            pgEx.ConstraintName != null &&
            ExceptionMap.TryGetValue(pgEx.ConstraintName, out var exceptionFactory))
        {
            return exceptionFactory();
        }

        return fallbackException;
    }
}