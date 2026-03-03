using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CompuTrabajo.Redarbor.Application.Command.CommandHandlers;
using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using CompuTrabajo.Redarbor.Domain.Common.Exceptions;
using System;

namespace CompuTrabajo.Redarbor.Application.Tests
{
    public class DeleteEmployeeCommandHandlerTests
    {
        [Fact]
        public async Task HandleAsync_Deletes_Employee_And_Executes_Transaction()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var existing = new Employee(employeeId, 1, "test@example.com", "pwd", 1, 1, 1, "user");

            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<DeleteEmployeeCommandHandler>>().Object;
            var handler = new DeleteEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new DeleteEmployeeCommand(employeeId);

            // Act
            await handler.HandleAsync(cmd, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.GetAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
            mockRepo.Verify(r => r.DeleteAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
            mockUow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_When_Employee_Already_Deleted_Throws_DomainException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var existing = new Employee(employeeId, 1, "test@example.com", "pwd", 1, 1, 1, "user");
            existing.SetDeletionDate(DateTime.UtcNow);

            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(existing);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<DeleteEmployeeCommandHandler>>().Object;
            var handler = new DeleteEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new DeleteEmployeeCommand(employeeId);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(cmd, CancellationToken.None));
        }
    }
}
