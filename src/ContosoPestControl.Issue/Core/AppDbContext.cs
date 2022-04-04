using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ContosoPestControl.Issue.Core
{
	public class AppDbContext : DbContext, IAppDbContext, IHealthCheck
	{
		private readonly ILogger<AppDbContext> _logger;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			_logger = logger;
		}

		public DbSet<Issue> Issues { get; set; }

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				await Database.ExecuteSqlRawAsync("SELECT 1");
				return HealthCheckResult.Healthy();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Health check failure.");
				return HealthCheckResult.Unhealthy();
			}
		}

		public async Task<Issue?> GetIssueAsync(Guid id)
		{
			return await Issues.SingleOrDefaultAsync(x => x.Id == id && x.Active);
		}
	}
}
