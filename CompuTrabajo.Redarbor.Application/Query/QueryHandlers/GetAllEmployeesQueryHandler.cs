using CompuTrabajo.Redarbor.Application.Common;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using Microsoft.Extensions.Logging;

namespace CompuTrabajo.Redarbor.Application.Query.QueryHandlers
{
    public class GetAllEmployeesQueryHandler : IQueryHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeReadDto>>
    {
        private readonly ILogger<GetAllEmployeesQueryHandler> _logger;
        private readonly IReadRepository<EmployeeReadDto> _readRepository;
        public GetAllEmployeesQueryHandler(IReadRepository<EmployeeReadDto> readRepository, ILogger<GetAllEmployeesQueryHandler> logger)
        {
            _logger = logger;
            _readRepository = readRepository;

        }
        public async Task<IReadOnlyList<EmployeeReadDto>> Handle(GetAllEmployeesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Returning all employees");
            return await _readRepository.GetAllAsync(cancellationToken);
        }
    }
}
