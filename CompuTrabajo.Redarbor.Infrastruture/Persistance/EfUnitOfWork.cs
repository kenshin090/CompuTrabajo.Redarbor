using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly RedarborDbContext _db;

        public EfUnitOfWork(RedarborDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct)
        {
            if (_db.Database.CurrentTransaction is not null)
            {
                await action(ct);
                return;
            }

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await action(ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
