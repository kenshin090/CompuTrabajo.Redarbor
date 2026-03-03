using System;
using Xunit;
using CompuTrabajo.Redarbor.Domain.Employees;
using CompuTrabajo.Redarbor.Domain.Common.Exceptions;

namespace CompuTrabajo.Redarbor.Domain.Test.Employees
{
    public class EmployeeTests
    {
        [Fact]
        public void Constructor_InvalidEmail_ThrowsDomainException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Employee(id, 1, "invalid-email", "pwd", 1, 1, 1, "user")
            );
        }

        [Fact]
        public void CanDelete_AlreadyDeleted_ThrowsDomainException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var employee = new Employee(id, 1, "test@example.com", "pwd", 1, 1, 1, "user");
            employee.SetDeletionDate(DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<DomainException>(() => employee.CanDelete());
        }
    }
}
