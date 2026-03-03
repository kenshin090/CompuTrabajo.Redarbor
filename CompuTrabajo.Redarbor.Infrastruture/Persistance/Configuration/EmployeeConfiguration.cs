using CompuTrabajo.Redarbor.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Configuration
{
   

    public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> b)
        {
            b.ToTable("Employees");
            b.HasKey(x => x.Id);

            b.Property(x => x.CompanyId).IsRequired();
            b.Property(x => x.Email).IsRequired();
            b.Property(x => x.Password).IsRequired();
            b.Property(x => x.PortalId).IsRequired();
            b.Property(x => x.RoleId).IsRequired();
            b.Property(x => x.StatusId).IsRequired();
            b.Property(x => x.UserName).IsRequired();
            b.Property(x => x.UserName);

            


            
        }
    }
}
