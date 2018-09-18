using Microsoft.EntityFrameworkCore;
using tutorials_datinApp.Api.Models;

namespace tutorials_datinApp.Api.Data
{
    public class DataContext: DbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users {get; set; }
    }
}