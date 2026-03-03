using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Application.Common.Interfaces
{
    public interface ICommand
    {
        public Guid CorrelationId { get; set; }
    }
}
