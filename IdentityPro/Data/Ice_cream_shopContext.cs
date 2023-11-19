using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdentityPro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityPro.Data
{
    public class Ice_cream_shopContext : DbContext
    {
        public Ice_cream_shopContext (DbContextOptions<Ice_cream_shopContext> options)
            : base(options)
        {
        }

        public DbSet<IdentityPro.Models.Product> Product { get; set; } = default!;

        public DbSet<IdentityPro.Models.User>? User { get; set; }

        public DbSet<IdentityPro.Models.Order>? Order { get; set; }
        public DbSet<IdentityPro.Models.OrderItem> OrderItem { get; set; } // Add this line

    }
}
