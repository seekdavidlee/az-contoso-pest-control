using Microsoft.EntityFrameworkCore;

namespace ContosoPestControl.Issue.Core
{
	public interface IAppDbContext
	{
		Task<Issue?> GetIssueAsync(Guid id);
		DbSet<Issue> Issues { get; set; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
