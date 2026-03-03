using CompuTrabajo.Redarbor.Application.Common;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Application.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Application.Query.QueryHandlers
{
    public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, EmployeeReadDto>
    {
        private readonly ILogger<GetEmployeeQueryHandler> _logger;
        private readonly IReadRepository<EmployeeReadDto> _readRepository;
        public GetEmployeeQueryHandler(IReadRepository<EmployeeReadDto> readRepository, ILogger<GetEmployeeQueryHandler> logger)
        {
            _logger = logger;
            _readRepository = readRepository;

        }
        public async Task<EmployeeReadDto> Handle(GetEmployeeQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Returning all employees");
            return await _readRepository.GetByIdAsync(query.EmployeeId,cancellationToken);
        }
    }
}
