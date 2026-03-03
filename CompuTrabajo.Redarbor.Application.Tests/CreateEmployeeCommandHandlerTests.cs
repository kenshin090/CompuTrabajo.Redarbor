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
    public class CreateEmployeeCommandHandlerTests
    {
        [Fact]
        public async Task HandleAsync_Adds_New_Employee_And_Executes_Transaction()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<CreateEmployeeCommandHandler>>().Object;
            var handler = new CreateEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new CreateEmployeeCommand
            {
                CompanyId = 1,
                Email = "new@example.com",
                Password = "password",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                UserName = "newuser"
            };

            // Act
            await handler.HandleAsync(cmd, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.AddAsync(It.Is<Employee>(e => e.Id == cmd.EmployeeId), It.IsAny<CancellationToken>()), Times.Once);
            mockUow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_When_Repository_Throws_Exception_Propagates()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Employee>>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("db error"));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

            var logger = new Mock<ILogger<CreateEmployeeCommandHandler>>().Object;
            var handler = new CreateEmployeeCommandHandler(mockRepo.Object, mockUow.Object, logger);

            var cmd = new CreateEmployeeCommand
            {
                CompanyId = 1,
                Email = "new@example.com",
                Password = "password",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                UserName = "newuser"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(cmd, CancellationToken.None));
        }
    }
}