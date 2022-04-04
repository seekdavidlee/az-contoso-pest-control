namespace ContosoPestControl.Issue.Core
{
	public interface IIssueService
	{
		Task<GetIssueModel?> GetAsync(Guid id);
		Task DeleteAsync(Guid id);
		Task UpdateAsync(Guid id, UpdateIssueModel model);
		Task ResolveAsync(Guid id, ResolveIssueModel model);
		Task<Guid> AddAsync(CreateIssueModel model);
	}
}
