using Microsoft.EntityFrameworkCore;

namespace TelemetryHub.Server.Data
{
  public class TelemetryContext : DbContext
  {
    public TelemetryContext(DbContextOptions<TelemetryContext> opts) : base(opts) { }
    public DbSet<Event> Events => Set<Event>();
  }
}
