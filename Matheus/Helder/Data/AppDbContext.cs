using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Folha> Folhas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseSqlite("Data Source=matheus_helder.db");
    }
}