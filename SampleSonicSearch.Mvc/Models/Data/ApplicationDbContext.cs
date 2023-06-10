using Microsoft.EntityFrameworkCore;
using SampleSonicSearch.Mvc.Models.Entites;

namespace SampleSonicSearch.Mvc.Models.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Car> Cars { get; set; }
  }
}
