namespace CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance
{

    public interface IRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Update(TEntity entity);
        Task DeleteAsync(Guid entityId, CancellationToken ct);
        Task<TEntity> GetAsync(Guid entityId, CancellationToken ct);
    }
}
