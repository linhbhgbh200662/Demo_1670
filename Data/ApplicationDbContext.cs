using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_1670.Models;

namespace Web_1670.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Book { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}