using CompuTrabajo.Redarbor.Application.Common.Dto;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Repository
{
    public class EmployeeReadRepository : IReadRepository<EmployeeReadDto>
    {
        private readonly string _cs;

        public EmployeeReadRepository(IConfiguration config)
            => _cs = config.GetConnectionString("Default")!;

        private IDbConnection CreateConnection() => new SqlConnection(_cs);

        public async Task<EmployeeReadDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
            SELECT  Id, CompanyId, Email, Name, Telephone, CreatedOn
            FROM    Employees WITH(NOLOCK)
            WHERE   Id = @Id
                AND DeletedOn IS NULL;
            """;

            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<EmployeeReadDto>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        }

        public async Task<IReadOnlyList<EmployeeReadDto>> GetAllAsync(CancellationToken ct)
        {
            const string sql = """
            SELECT  Id, CompanyId, Email, Name, Telephone, CreatedOn
            FROM    Employees  WITH(NOLOCK)          
              WHERE  DeletedOn IS NULL
            ORDER BY CreatedOn DESC;
            """;

            using var conn = CreateConnection();
            var rows = await conn.QueryAsync<EmployeeReadDto>(
                new CommandDefinition(sql, cancellationToken: ct));

            return rows.AsList();
        }
    }
}
