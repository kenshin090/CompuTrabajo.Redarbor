using CompuTrabajo.Redarbor.Application.Common;

namespace CompuTrabajo.Redarbor.Application.Query
{
    public class GetEmployeeQuery : IQuery
    {
       

        public GetEmployeeQuery(Guid id)
        {
            this.EmployeeId = id;
        }

        public Guid EmployeeId { get; set; }
    }
}
