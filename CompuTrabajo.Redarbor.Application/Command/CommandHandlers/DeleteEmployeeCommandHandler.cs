using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common.Interfaces;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Domain.Employees;
using Microsoft.Extensions.Logging;

namespace CompuTrabajo.Redarbor.Application.Command.CommandHandlers
{
    public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand>
    {


        private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Employee> _employeeRepository;
        public DeleteEmployeeCommandHandler(
            IRepository<Employee> employeeRepository,
            IUnitOfWork uow,
            ILogger<DeleteEmployeeCommandHandler> logger
            )
        {
            _logger = logger;
            _uow = uow;
            _employeeRepository = employeeRepository;
        }

        public async Task HandleAsync(DeleteEmployeeCommand command, CancellationToken cancellationToken)
        {
            await _uow.ExecuteInTransactionAsync(async token =>
            {
                _logger.LogInformation($"DeleteemployeeeComandHandler started with correlation id = {command.CorrelationId}");


                Employee existing = await _employeeRepository.GetAsync(command.EmployeeId,token);
                existing.CanDelete();
                await _employeeRepository.DeleteAsync(command.EmployeeId,token);

                _logger.LogInformation($"DeleteemployeeeComandHandler ended with correlation id = {command.CorrelationId}");
            }, cancellationToken);



        }


    }



}
