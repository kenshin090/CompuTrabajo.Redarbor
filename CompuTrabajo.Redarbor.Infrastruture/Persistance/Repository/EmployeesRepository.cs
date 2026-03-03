using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using CompuTrabajo.Redarbor.Infrastruture.Persistance.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Repository
{
    public class EmployeesRepository : IEmployeeRepository,IRepository<Employee>
    {
        private readonly RedarborDbContext _dbContext;
        private readonly ILogger<EmployeesRepository> _logger;
        public EmployeesRepository(RedarborDbContext dbContext, ILogger<EmployeesRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddAsync(Employee entity, CancellationToken ct)
        {
            _logger.LogInformation($"Adding new employee to the database with id {entity.Id}");
            await _dbContext.Employees.AddAsync(entity,ct);
            _logger.LogInformation($"Added to the context");
        }

        public async Task DeleteAsync(Guid entityId, CancellationToken ct)
        {
            _logger.LogInformation($"Deleting employee in the database with id {entityId}");
            Employee toDelete = _dbContext.Employees.FirstOrDefault(e => e.Id == entityId);
            if (toDelete == null)
                throw new PersistanceException($"Employee with id {entityId} dont exist");

            toDelete.SetDeletionDate(DateTime.UtcNow);

            
            _logger.LogInformation($"Employe deleted");
        }

        public async Task<Employee> GetAsync(Guid entityId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retreiving employee from the database with id {entityId}");
             Employee employe = await _dbContext.Employees.FirstOrDefaultAsync((e => e.Id == entityId),cancellationToken);
            if (employe == null)
                throw new PersistanceException($"Employee with id {entityId} dont exist");

            _logger.LogInformation($"Employee retrieved");

            return employe;
        }

        public void Update(Employee entity)
        {
            _logger.LogInformation("Updating employee with id {EmployeeId}", entity.Id);

            _dbContext.Employees.Update(entity);

            _logger.LogInformation("Employee marked as modified in context");
        }
    }
}
