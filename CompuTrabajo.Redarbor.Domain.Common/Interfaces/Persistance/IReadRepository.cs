using CompuTrabajo.Redarbor.Application.Common.Dto;

namespace CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance
{
    public interface IReadRepository<T> where T : IReadEntityDto
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
    }
}
