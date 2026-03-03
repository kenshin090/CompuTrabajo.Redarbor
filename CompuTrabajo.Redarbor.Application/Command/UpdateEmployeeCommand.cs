using CompuTrabajo.Redarbor.Application.Common.Interfaces;

namespace CompuTrabajo.Redarbor.Application.Command
{
    public class UpdateEmployeeCommand : ICommand
    {
        public Guid EmployeeId { get; set; }
        public required int CompanyId { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public required string Password { get; set; }
        public  int RoleId { get; set; }
        public  int StatusId { get; set; }
        public string? Telephone { get;  set; }
        public required string UserName { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
