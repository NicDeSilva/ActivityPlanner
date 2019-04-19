using Microsoft.EntityFrameworkCore;
 
namespace belt.Models
{
    public class DojoContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public DojoContext(DbContextOptions options) : base(options) { }
         public DbSet<User> Users {get;set;}
         public DbSet<Plan> Plans {get;set;}
         public DbSet<Activ> Activities {get;set;}
    }
}