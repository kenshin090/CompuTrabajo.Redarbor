using CompuTrabajo.Redarbor.Domain.Common.Exceptions;
using CompuTrabajo.Redarbor.Infrastructure.Common.Interceptor;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CompuTrabajo.Redarbor.Domain.Employees
{
    public sealed class Employee : IAuditable
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex ColombianMobileRegex = new(@"^3\d{9}$", RegexOptions.Compiled);
        private static readonly Regex NameRegex = new(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{2,100}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        public Guid Id { get; private set; }
        public int CompanyId { get; set; }        
        public DateTime? DeletedOn { get; private set; }
        public  string Email { get; private set; }
        public string? Name { get; private set; }
        public DateTime? Lastlogin { get; private set; }
        public  string Password { get; private set; }
        public  int PortalId { get; private set; }
        public  int RoleId { get; private set; }
        public  int StatusId { get; private set; }
        public string? Telephone { get; private set; }
        

        public string UserName { get; private set; }
        public DateTime CreatedOn { get;  set; }
        public DateTime UpdatedOn { get;  set; }

        public Employee(Guid EmployeeId, int companyId, string email, string password, int portalId, int roleId, int statusId, string userName)
        {
            Id = EmployeeId;
            CompanyId = companyId;
            ValidateEmail(email);
            Email = email;
            Password = password;
            PortalId = portalId;
            RoleId = roleId;
            StatusId = statusId;
            UserName = userName;
        }

        // Parameterless constructor required by EF Core for materialization
        // Keep it protected to avoid direct usage from application code.
        protected Employee() { }

        private static void ValidateEmail(string email)
        {            
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");

            if (!EmailRegex.IsMatch(email))
                throw new DomainException("Email format is invalid.");

        }

        public void SetPhoneTelephone(string telephone)
        {
            telephone.Trim();
            ValidateTelephone(telephone);
            Telephone = telephone;
        }

        public void SetDeletionDate(DateTime deletedDate)
        {
            DeletedOn = deletedDate;
        }

        public void SetName( string name)
        {
            name.Trim();
            ValidateName(name);
            Name = name;
        }

        private static void ValidateName(string name) {
            if (!NameRegex.IsMatch(name))
                throw new DomainException("Name format is invalid.");
        }

        private static void ValidateTelephone(string telephone) {

            if (!ColombianMobileRegex.IsMatch(telephone))
                throw new DomainException("Telephone format is invalid.");

        }

        public void CanDelete()
        {
            if (DeletedOn.HasValue)
                throw new DomainException($"Employee deleted previously on date {DeletedOn.Value}.");
        }
    }
}

