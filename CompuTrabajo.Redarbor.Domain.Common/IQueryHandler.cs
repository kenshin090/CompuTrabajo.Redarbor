using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Application.Common
{
    public interface IQueryHandler<T,TEntity> where T : IQuery
    {
        Task<TEntity> Handle(T query, CancellationToken cancellationToken);
    }
}
