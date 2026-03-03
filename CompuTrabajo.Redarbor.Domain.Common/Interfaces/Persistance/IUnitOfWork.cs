using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);

    }
}
