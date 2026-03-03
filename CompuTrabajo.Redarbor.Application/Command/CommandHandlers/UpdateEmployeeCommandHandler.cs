using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common.Interfaces;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using Microsoft.Extensions.Logging;

namespace CompuTrabajo.Redarbor.Application.Command.CommandHandlers
{
    public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand>
    {
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Employee> _employeeRepository;
        public UpdateEmployeeCommandHandler(
            IRepository<Employee> employeeRepository,
            IUnitOfWork uow,
            ILogger<UpdateEmployeeCommandHandler> logger
            )
        {
            _logger = logger;
            _uow = uow;
            _employeeRepository = employeeRepository;
        }

        public async Task HandleAsync(UpdateEmployeeCommand command, CancellationToken cancellationToken)
        {
            await _uow.ExecuteInTransactionAsync(async token =>
            {
                _logger.LogInformation($"updateemployeeeComandHandler started with correlation id = {command.CorrelationId}");


                Employee existing = await _employeeRepository.GetAsync(command.EmployeeId,token);


                if (!string.IsNullOrWhiteSpace(command.Telephone) && (command.Telephone != existing.Telephone))
                    existing.SetPhoneTelephone(command.Telephone);

                if (!string.IsNullOrWhiteSpace(command.Name) && (command.Name != existing.Name))
                    existing.SetName(command.Name);

                _employeeRepository.Update(existing);

                _logger.LogInformation($"UpdateemployeeeComandHandler ended with correlation id = {command.CorrelationId}");
            }, cancellationToken);



        }
    }
}
