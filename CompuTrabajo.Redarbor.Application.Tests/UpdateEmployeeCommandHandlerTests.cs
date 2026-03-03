using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CompuTrabajo.Redarbor.Application.Command.CommandHandlers;
using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using System;

namespace CompuTrabajo.Redarbor.Application.Tests
{
    public class UpdateEmployeeCommandHandlerTests
    {
        [Fact]
        public async Task HandleAsync_Updates_Employee_And_Executes_Transaction()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var existing = new Employee(employeeId, 1, "test@example.com", "pwd", 1, 1, 1, "user");

            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            mockRepo.Setup(r => r.Update(It.IsAny<Employee>()));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<UpdateEmployeeCommandHandler>>().Object;
            var handler = new UpdateEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new UpdateEmployeeCommand
            {
                EmployeeId = employeeId,
                CompanyId = 1,
                Email = "test@example.com",
                Password = "pwd",
                RoleId = 1,
                StatusId = 1,
                UserName = "user"
            };

            // Act
            await handler.HandleAsync(cmd, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.GetAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
            mockRepo.Verify(r => r.Update(It.IsAny<Employee>()), Times.Once);
            mockUow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_When_GetAsync_Throws_Exception_Propagates()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("repo failure"));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<UpdateEmployeeCommandHandler>>().Object;
            var handler = new UpdateEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new UpdateEmployeeCommand { EmployeeId = employeeId, CompanyId = 1, Password = "pwd", RoleId = 1, StatusId = 1, UserName = "user", Email = "a@b.com" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(cmd, CancellationToken.None));
        }
    }
}
