using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Stock> Stocks { get; set; }
        public DbSet<Models.Comment> Comments { get; set; }
        public DbSet<Models.Portfolio> Portfolios { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Portfolio>().HasKey(p => new { p.AppUserId, p.StockId });
            modelBuilder.Entity<Portfolio>().HasOne(p => p.AppUser).WithMany(u => u.Portfolios).HasForeignKey(p => p.AppUserId);
            modelBuilder.Entity<Portfolio>().HasOne(p => p.Stock).WithMany(s => s.Portfolios).HasForeignKey(p => p.StockId);
            modelBuilder.Entity<Stock>().HasMany(s => s.Comments).WithOne(c => c.Stock).OnDelete(DeleteBehavior.Cascade);

            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new() {Name="Admin", NormalizedName="ADMIN"},
                new() {Name="User", NormalizedName="USER"}
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}