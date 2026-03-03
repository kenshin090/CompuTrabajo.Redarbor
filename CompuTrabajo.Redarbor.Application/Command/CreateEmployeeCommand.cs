using CompuTrabajo.Redarbor.Application.Common.Interfaces;

namespace CompuTrabajo.Redarbor.Application.Command
{
    public class CreateEmployeeCommand : ICommand
    {
        public CreateEmployeeCommand()
        {
            this.CorrelationId = Guid.NewGuid();
            this.EmployeeId = Guid.NewGuid();
        }

        public Guid EmployeeId { get; private set; }
        public int CompanyId { get; set; }
        // Required for creation - keep non-nullable and provide default to satisfy the compiler
        public string Email { get; set; } = default!;
        // Optional fields should be nullable so model binding does not mark them as required
        public string? Name { get; set; }
        public string Password { get; set; } = default!;
        public int PortalId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public string? Telephone { get; set; }
        public string UserName { get; set; } = default!;
        public Guid CorrelationId { get; set; }
        
    }
}
