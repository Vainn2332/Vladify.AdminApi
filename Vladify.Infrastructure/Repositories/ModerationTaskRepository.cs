using Dapper;
using Microsoft.EntityFrameworkCore;
using Vladify.Application.Interfaces;
using Vladify.Application.Models;
using Vladify.Domain.Entities;

namespace Vladify.Infrastructure.Repositories;

public class ModerationTaskRepository(ApplicationDbContext context) : IModerationTaskRepository
{
    public async Task<ModerationTask> CreateAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        context.ModerationTasks.Add(task);

        await context.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<List<ModerationTask>> GetAllAsync(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();

        int offset = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
        var query = """
            SELECT * FROM "ModerationTasks"
            ORDER BY "Id"
            LIMIT @Limit OFFSET @Offset
            """;

        var command = new CommandDefinition(
            query,
            new { Limit = paginationFilter.PageSize, Offset = offset },
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<ModerationTask>(command);

        return result.AsList();
    }

    public Task<ModerationTask?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();

        var query = """
            SELECT * FROM "ModerationTasks"
            Where "Id" =@Id
            """;

        var command = new CommandDefinition(
            query,
            new { Id = id },
            cancellationToken: cancellationToken);

        return connection.QueryFirstOrDefaultAsync<ModerationTask>(command);
    }

    public async Task<ModerationTask> UpdateAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        context.ModerationTasks.Update(task);

        await context.SaveChangesAsync(cancellationToken);

        return task;
    }

    public Task DeleteAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        context.ModerationTasks.Remove(task);

        return context.SaveChangesAsync(cancellationToken);
    }
}
