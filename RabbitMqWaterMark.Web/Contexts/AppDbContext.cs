using Microsoft.EntityFrameworkCore;
using RabbitMqWaterMark.Web.Models;

namespace RabbitMqWaterMark.Web.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
