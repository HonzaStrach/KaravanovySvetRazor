using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Models;

namespace KaravanovySvet.Data
{
    public class KaravanovySvetContext : DbContext
    {
        public KaravanovySvetContext (DbContextOptions<KaravanovySvetContext> options)
            : base(options)
        {
        }

        public DbSet<KaravanovySvet.Models.Blogs> Blog { get; set; } = default!;
        public DbSet<KaravanovySvet.Models.BlogImage> BlogImage { get; set; } = default!;
    }
}
