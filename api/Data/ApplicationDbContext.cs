using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationDbContext : IdentityDbContext
// DbContext was early
{
    public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Comment> Comment { get; set; }
}