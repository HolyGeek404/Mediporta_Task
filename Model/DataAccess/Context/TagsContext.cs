using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;

namespace Model.DataAccess.Context;

public class TagsContext(IConfiguration configuration) : DbContext()
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration.GetConnectionString("Local"));
    }
    
    public DbSet<Tag> Tags { get; set; }
}