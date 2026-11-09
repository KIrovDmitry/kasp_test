using Kirov_DA_test.Controllers;
using Kirov_DA_test.Models;
using Kirov_DA_test.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Kirov_DA_test.Tests;

public class ScanTasksControllerTests
{
    private readonly Mock<IScanTaskStore> _mockTaskStore;
    private readonly ScanTasksController _controller;

    public ScanTasksControllerTests()
    {
        _mockTaskStore = new Mock<IScanTaskStore>();

        // Передаем null вместо ScanBackgroundService, так как эти тесты не используют его
        _controller = new ScanTasksController(_mockTaskStore.Object, null!);
    }

    [Fact]
    public async Task GetScanStatus_TaskExists_ReturnsOkResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new ScanTask
        {
            Id = taskId,
            RepositoryUrl = "https://gitlab.com/example/repo",
            Status = "Running"
        };

        _mockTaskStore.Setup(x => x.GetByIdAsync(taskId))
                      .ReturnsAsync(existingTask);

        // Act
        var result = await _controller.GetScanStatus(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedTask = Assert.IsType<ScanTask>(okResult.Value);
        Assert.Equal(taskId, returnedTask.Id);
        Assert.Equal("Running", returnedTask.Status);
    }

    [Fact]
    public async Task GetScanStatus_TaskNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var randomId = Guid.NewGuid();
        _mockTaskStore.Setup(x => x.GetByIdAsync(randomId))
                      .ReturnsAsync((ScanTask?)null);

        // Act
        var result = await _controller.GetScanStatus(randomId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}