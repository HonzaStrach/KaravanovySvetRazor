using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KaravanovySvet.Data
{
    public class KaravanovySvetContext : IdentityDbContext
    {
        public KaravanovySvetContext (DbContextOptions<KaravanovySvetContext> options)
            : base(options)
        {
        }

        public DbSet<KaravanovySvet.Models.Blogs> Blog { get; set; } = default!;
        public DbSet<KaravanovySvet.Models.BlogImage> BlogImage { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set the default schema
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}
