using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> TaskItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}