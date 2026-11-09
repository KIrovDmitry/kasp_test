using Kirov_DA_test.Controllers;
using Kirov_DA_test.Models;
using Kirov_DA_test.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Kirov_DA_test.Tests;

public class VulnerabilitiesControllerTests
{
    private readonly Mock<IVulnerabilityStore> _mockStore;
    private readonly VulnerabilitiesController _controller;

    public VulnerabilitiesControllerTests()
    {
        // Инициализируем мок (имитацию) хранилища перед каждым тестом
        _mockStore = new Mock<IVulnerabilityStore>();
        // Передаем мок в контроллер
        _controller = new VulnerabilitiesController(_mockStore.Object);
    }

    [Fact]
    public async Task CreateVulnerability_ValidData_ReturnsCreatedAtActionResult()
    {
        // Arrange (Подготовка)
        var testVulnerability = new Vulnerability
        {
            Name = "Test XSS",
            severity = "High"
        };

        // Настраиваем мок: при вызове AddAsync вернуть переданный объект
        _mockStore.Setup(x => x.AddAsync(It.IsAny<Vulnerability>()))
                  .ReturnsAsync((Vulnerability v) => v);

        // Act (Действие)
        var result = await _controller.CreateVulnerability(testVulnerability);

        // Assert (Проверка)
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.NotNull(createdAtResult.Value);

        var returnedVulnerability = Assert.IsType<Vulnerability>(createdAtResult.Value);
        Assert.Equal("Test XSS", returnedVulnerability.Name);
        Assert.Equal("High", returnedVulnerability.severity);
    }

    [Fact]
    public async Task GetVulnerability_NotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var randomId = Guid.NewGuid();
        // Настраиваем мок возвращать null, если уязвимость не найдена
        _mockStore.Setup(x => x.GetByIdAsync(randomId)).ReturnsAsync((Vulnerability?)null);

        // Act
        var result = await _controller.GetVulnerability(randomId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
