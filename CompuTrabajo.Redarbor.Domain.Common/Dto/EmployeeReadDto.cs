using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;

namespace CompuTrabajo.Redarbor.Application.Common.Dto
{
    public sealed class EmployeeReadDto : IReadEntityDto
    {
        public Guid Id { get; init; }
        public int CompanyId { get; init; }
        public string Email { get; init; } = default!;
        public string? Name { get; init; }
        public string? Telephone { get; init; }
        public DateTime CreatedOn { get; init; }
    }
}
