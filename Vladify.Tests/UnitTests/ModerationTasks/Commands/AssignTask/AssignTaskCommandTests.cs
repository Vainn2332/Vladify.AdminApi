using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Vladify.Application.Commands.ModerationTasks.AssignTask;
using Vladify.Application.Interfaces;

namespace Vladify.Tests.UnitTests.ModerationTasks.Commands.AssignTask;

public class AssignTaskCommandTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IModerationTaskRepository> _repositoryMock;
    private readonly AssignTaskCommandHandler _handler;

    public AssignTaskCommandTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<IModerationTaskRepository>>();
        _handler = _fixture.Create<AssignTaskCommandHandler>();
    }

    [Fact]
    public async Task Handle_ShouldReturnTaskId_WhenTaskIsSuccessfullyClaimed()
    {
        var command = _fixture.Create<AssignTaskCommand>();
        var expectedTaskId = _fixture.Create<Guid>();

        _repositoryMock
            .Setup(repo => repo.ClaimNextPendingTaskAsync(command.ModeratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTaskId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull()
            .And.Be(expectedTaskId);

        _repositoryMock.Verify(repo => repo.ClaimNextPendingTaskAsync(command.ModeratorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNoPendingTasksInQueue()
    {
        var command = _fixture.Create<AssignTaskCommand>();

        _repositoryMock
            .Setup(repo => repo.ClaimNextPendingTaskAsync(command.ModeratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();

        _repositoryMock.Verify(repo => repo.ClaimNextPendingTaskAsync(command.ModeratorId, It.IsAny<CancellationToken>()), Times.Once);
    }
}