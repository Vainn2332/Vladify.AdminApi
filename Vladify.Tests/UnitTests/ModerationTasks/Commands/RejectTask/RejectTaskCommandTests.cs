using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Vladify.Application.Commands.ModerationTasks.RejectTask;
using Vladify.Application.Exceptions;
using Vladify.Application.Interfaces;
using Vladify.Domain.Entities;
using Vladify.Domain.Enums;

namespace Vladify.Tests.UnitTests.ModerationTasks.Commands.RejectTask;

public class RejectTaskCommandTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IModerationTaskRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RejectTaskCommandHandler _handler;

    public RejectTaskCommandTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<IModerationTaskRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _handler = _fixture.Create<RejectTaskCommandHandler>();
    }

    [Fact]
    public async Task Handle_ShouldRejectTaskAndReturnResponse_WhenTaskExistsAndAssignedToCorrectModerator()
    {
        var command = _fixture.Create<RejectTaskCommand>();

        var existingTask = _fixture.Build<ModerationTask>()
            .With(t => t.Id, command.TaskId)
            .With(t => t.AssignedModeratorId, command.ModeratorId)
            .With(t => t.Status, ModerationStatus.Pending)
            .Without(t => t.Message)
            .Create();

        var updatedTask = _fixture.Create<ModerationTask>();
        var expectedResponse = _fixture.Create<RejectedTaskResponse>();

        _repositoryMock
            .Setup(repo => repo.GetAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        _mapperMock
            .Setup(m => m.Map<RejectedTaskResponse>(updatedTask))
            .Returns(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull()
            .And.Be(expectedResponse);

        existingTask.Status.Should().Be(ModerationStatus.Rejected);
        existingTask.Message.Should().Be(command.RejectionReason);
        existingTask.ResolvedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(repo => repo.GetAsync(command.TaskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateAsync(existingTask, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        var command = _fixture.Create<RejectTaskCommand>();

        _repositoryMock
            .Setup(repo => repo.GetAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModerationTask?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowTaskAssignedToDifferentModeratorException_WhenTaskIsAssignedToAnotherModerator()
    {
        var command = _fixture.Create<RejectTaskCommand>();
        var differentModeratorId = _fixture.Create<Guid>();

        var existingTask = _fixture.Build<ModerationTask>()
            .With(t => t.Id, command.TaskId)
            .With(t => t.AssignedModeratorId, differentModeratorId)
            .Create();

        _repositoryMock
            .Setup(repo => repo.GetAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<TaskAssignedToDifferentModeratorException>();

        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowTaskNotClaimedException_WhenTaskHasNoAssignedModerator()
    {
        var command = _fixture.Create<RejectTaskCommand>();

        var existingTask = _fixture.Build<ModerationTask>()
            .With(t => t.Id, command.TaskId)
            .With(t => t.AssignedModeratorId, (Guid?)null)
            .Create();

        _repositoryMock
            .Setup(repo => repo.GetAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<TaskNotClaimedException>();

        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
