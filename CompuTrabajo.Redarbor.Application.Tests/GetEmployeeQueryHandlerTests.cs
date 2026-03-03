using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CompuTrabajo.Redarbor.Application.Query.QueryHandlers;
using CompuTrabajo.Redarbor.Application.Query;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using System;

namespace CompuTrabajo.Redarbor.Application.Tests
{
    public class GetEmployeeQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_Employee_By_Id()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new EmployeeReadDto { Id = id, CompanyId = 1, Email = "test@example.com", CreatedOn = DateTime.UtcNow };

            var mockRepo = new Mock<IReadRepository<EmployeeReadDto>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g == id), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var logger = new Mock<ILogger<GetEmployeeQueryHandler>>().Object;
            var handler = new GetEmployeeQueryHandler(mockRepo.Object, logger);

            // Act
            var result = await handler.Handle(new GetEmployeeQuery(id), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
        }

        [Fact]
        public async Task Handle_When_Repository_Throws_Exception_Propagates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockRepo = new Mock<IReadRepository<EmployeeReadDto>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("repo failure"));

            var logger = new Mock<ILogger<GetEmployeeQueryHandler>>().Object;
            var handler = new GetEmployeeQueryHandler(mockRepo.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetEmployeeQuery(id), CancellationToken.None));
        }
    }
}