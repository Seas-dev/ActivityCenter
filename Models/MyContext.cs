using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class MyContext : DbContext
    {
        public MyContext (DbContextOptions options) : base(options) {}

        // add project models here
        public DbSet<User> Users {get;set;}
        public DbSet<DojoActivity> DojoActivities {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}