using Microsoft.EntityFrameworkCore;
using DocAssistant.Api.Models;

namespace DocAssistant.Api.Data;
public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Document> Documents { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}