using CompuTrabajo.Redarbor.Application.Common.Interfaces;

namespace CompuTrabajo.Redarbor.Application.Command
{
    public class DeleteEmployeeCommand : ICommand
    {
        public DeleteEmployeeCommand(Guid id)
        {
            this.EmployeeId = id;
            this.CorrelationId = Guid.NewGuid();
        }

        public Guid EmployeeId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
