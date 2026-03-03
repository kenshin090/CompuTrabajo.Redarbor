using CompuTrabajo.Redarbor.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance
{
    public class RedarborDbContext : DbContext
    {

        public RedarborDbContext(DbContextOptions<RedarborDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RedarborDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
