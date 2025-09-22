namespace VybeCheck.Models;

using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    //public DbSet<User> Users { get; set; }
    
    //public DbSet<Album> Albums { get; set; }

    //public DbSet<Comment> Comments { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}