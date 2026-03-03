using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common.Interfaces;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using Microsoft.Extensions.Logging;


namespace CompuTrabajo.Redarbor.Application.Command.CommandHandlers
{
    public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand>
    {
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Employee> _employeeRepository;
        public CreateEmployeeCommandHandler(
            IRepository<Employee> employeeRepository,
            IUnitOfWork uow,
            ILogger<CreateEmployeeCommandHandler> logger
            )
        {
            _logger = logger;
            _uow = uow;
            _employeeRepository = employeeRepository;
        }

        public async Task HandleAsync(CreateEmployeeCommand command, CancellationToken cancellationToken)
        {
            await _uow.ExecuteInTransactionAsync(async token => {
                _logger.LogInformation($"Create employeeeComandHandler started with correlation id = {command.CorrelationId}");

                Employee newEmploye = new Employee(command.EmployeeId,command.CompanyId, command.Email, command.Password, command.PortalId, command.RoleId, command.StatusId, command.UserName);

                if (!string.IsNullOrWhiteSpace(command.Telephone))
                    newEmploye.SetPhoneTelephone(command.Telephone);

                if (!string.IsNullOrWhiteSpace(command.Name))
                    newEmploye.SetName(command.Name);

                await _employeeRepository.AddAsync(newEmploye,token);

                _logger.LogInformation($"Create employeeeComandHandler ended with correlation id = {command.CorrelationId}");
            },cancellationToken);

            

        }
    }
}
