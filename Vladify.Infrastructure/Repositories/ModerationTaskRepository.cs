using Dapper;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Registry;
using Vladify.Application.Interfaces;
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

    public async Task<Guid?> ClaimNextPendingTaskAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async pollyCancellationToken =>
        {
            var connection = context.Database.GetDbConnection();

            var query = """
            UPDATE "ModerationTasks"
            SET "AssignedModeratorId" = @ModeratorId
            WHERE "Id" = (
                SELECT "Id" 
                FROM "ModerationTasks" 
                WHERE "AssignedModeratorId" IS NULL AND "Status" = 'Pending' 
                ORDER BY "CreatedAt" ASC 
                LIMIT 1 
                FOR UPDATE SKIP LOCKED
            )
            RETURNING "Id"
            """;

            var command = new CommandDefinition(
                query,
                new { ModeratorId = moderatorId },
                cancellationToken: pollyCancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Guid?>(command);

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

    public async Task<bool> HasActiveTaskAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async pollyCancellationToken =>
        {
            var connection = context.Database.GetDbConnection();

            var query = """
                SELECT EXISTS (
                SELECT 1 
                FROM "ModerationTasks" 
                WHERE "AssignedModeratorId" = @ModeratorId );
            """;

            var command = new CommandDefinition(
                query,
                new { Id = moderatorId },
                cancellationToken: pollyCancellationToken);

            return await connection.QueryFirstOrDefaultAsync<bool>(command);

        }, cancellationToken);
    }
}
