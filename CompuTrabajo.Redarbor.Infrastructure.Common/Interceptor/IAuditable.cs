namespace CompuTrabajo.Redarbor.Infrastructure.Common.Interceptor;
//Interface for define the audit fields on DB
public interface IAuditable
{
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }
}

