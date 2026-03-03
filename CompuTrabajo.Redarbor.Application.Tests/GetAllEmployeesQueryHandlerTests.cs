using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CompuTrabajo.Redarbor.Application.Query.QueryHandlers;
using CompuTrabajo.Redarbor.Application.Query;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using System.Collections.Generic;
using System;

namespace CompuTrabajo.Redarbor.Application.Tests
{
    public class GetAllEmployeesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_All_Employees()
        {
            // Arrange
            var mockRepo = new Mock<IReadRepository<EmployeeReadDto>>();
            var expected = new List<EmployeeReadDto>
            {
                new EmployeeReadDto { Id = Guid.NewGuid(), CompanyId = 1, Email = "a@b.com", CreatedOn = DateTime.UtcNow }
            };
            mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var logger = new Mock<ILogger<GetAllEmployeesQueryHandler>>().Object;
            var handler = new GetAllEmployeesQueryHandler(mockRepo.Object, logger);

            // Act
            var result = await handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Id, result[0].Id);
        }

        [Fact]
        public async Task Handle_When_Repository_Throws_Exception_Propagates()
        {
            // Arrange
            var mockRepo = new Mock<IReadRepository<EmployeeReadDto>>();
            mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("repo failure"));

            var logger = new Mock<ILogger<GetAllEmployeesQueryHandler>>().Object;
            var handler = new GetAllEmployeesQueryHandler(mockRepo.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None));
        }
    }
}