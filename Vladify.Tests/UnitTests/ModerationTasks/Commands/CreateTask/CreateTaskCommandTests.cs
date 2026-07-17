using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Vladify.Application.Commands.ModerationTasks.CreateTask;
using Vladify.Application.Interfaces;
using Vladify.Domain.Entities;
using Vladify.Domain.Enums;

namespace Vladify.Tests.UnitTests.ModerationTasks.Commands.CreateTask;

public class CreateTaskCommandTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IModerationTaskRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<IModerationTaskRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();

        _handler = _fixture.Create<CreateTaskCommandHandler>();
    }

    [Fact]
    public async Task Handle_ShouldCreateTaskAndReturnResponse_WhenCommandIsValid()
    {
        var command = _fixture.Create<CreateTaskCommand>();
        var mappedTask = _fixture.Create<ModerationTask>();
        var createdTask = _fixture.Create<ModerationTask>();
        var expectedResponse = _fixture.Create<CreatedTaskResponse>();

        _mapperMock
            .Setup(m => m.Map<ModerationTask>(command))
            .Returns(mappedTask);

        _repositoryMock
            .Setup(repo => repo.CreateAsync(mappedTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        _mapperMock
            .Setup(m => m.Map<CreatedTaskResponse>(createdTask))
            .Returns(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull()
            .And.Be(expectedResponse);

        mappedTask.Status.Should().Be(ModerationStatus.Pending);
        mappedTask.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(
            repo => repo.CreateAsync(mappedTask, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
