using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EstateInvestmentWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;

namespace EstateInvestmentWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<EstateCatalog> EstateCatalogs { get; set; }
        public DbSet<EstateProject> EstateProjects { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<NumberUserAccess> NumberUserAccesses { get; set; }


    }
}
