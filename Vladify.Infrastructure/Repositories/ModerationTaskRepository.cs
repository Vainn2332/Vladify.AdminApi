using Dapper;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Registry;
using Vladify.Application.Interfaces;
using Vladify.Application.Models;
using Vladify.Domain.Entities;
using Vladify.Infrastructure.Constants;

namespace Vladify.Infrastructure.Repositories;

public class ModerationTaskRepository(ApplicationDbContext context, ResiliencePipelineProvider<string> pipelineProvider) : IModerationTaskRepository
{
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline(PollyPipelineConstants.DbRetryPipelineName);

    public async Task<ModerationTask> CreateAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        context.ModerationTasks.Add(task);

        await context.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<List<ModerationTask>> GetAllAsync(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async pollyCancellationToken =>
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
                cancellationToken: pollyCancellationToken);

            var result = await connection.QueryAsync<ModerationTask>(command);

            return result.AsList();
        }, cancellationToken);
    }

    public async Task<ModerationTask?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async pollyCancellationToken =>
        {
            var connection = context.Database.GetDbConnection();

            var query = """
            SELECT * FROM "ModerationTasks"
            WHERE "Id" =@Id
            """;

            var command = new CommandDefinition(
                query,
                new { Id = id },
                cancellationToken: pollyCancellationToken);

            return await connection.QueryFirstOrDefaultAsync<ModerationTask?>(command);
        }, cancellationToken);
    }

    public async Task<ModerationTask> UpdateAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async pollyCancellationToken =>
        {
            context.ModerationTasks.Update(task);

            await context.SaveChangesAsync(pollyCancellationToken);

            return task;
        }, cancellationToken);
    }

    public async Task DeleteAsync(ModerationTask task, CancellationToken cancellationToken)
    {
        await _pipeline.ExecuteAsync(async pollyCancellationToken =>
        {
            context.ModerationTasks.Remove(task);

            await context.SaveChangesAsync(pollyCancellationToken);
        }, cancellationToken);
    }
}
