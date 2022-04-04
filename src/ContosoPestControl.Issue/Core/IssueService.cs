using AutoMapper;

namespace ContosoPestControl.Issue.Core
{
	public partial class IssueService : IIssueService
	{
		private readonly IAppDbContext _appDbContext;
		private readonly IMapper _mapper;
		private readonly IPubSubService _pubSubService;

		public IssueService(IAppDbContext appDbContext, IMapper mapper, IPubSubService pubSubService)
		{
			_appDbContext = appDbContext;
			_mapper = mapper;
			_pubSubService = pubSubService;
		}

		public async Task<GetIssueModel?> GetAsync(Guid id)
		{
			var issue = await _appDbContext.GetIssueAsync(id);
			return _mapper.Map<GetIssueModel>(issue);
		}

		public async Task DeleteAsync(Guid id)
		{
			var issue = await _appDbContext.GetIssueAsync(id);
			if (issue == null)
			{
				throw new ApplicationException("Issue id is invalid.");
			}

			if (issue.ResolutionDetailId != null)
			{
				throw new ApplicationException("You cannot remove an issue as it has been resolved.");
			}

			issue.Active = false;
			_appDbContext.Issues.Update(issue);
			await _appDbContext.SaveChangesAsync();
		}

		public async Task UpdateAsync(Guid id, UpdateIssueModel model)
		{
			var issue = await _appDbContext.GetIssueAsync(id);
			if (issue == null)
			{
				throw new ApplicationException("Issue id is invalid.");
			}

			if (issue.ResolutionDetailId != null)
			{
				throw new ApplicationException("You cannot update an issue as it has been resolved.");
			}

			_mapper.Map(model, issue);

			_appDbContext.Issues.Update(issue);
			await _appDbContext.SaveChangesAsync();
		}

		public async Task ResolveAsync(Guid id, ResolveIssueModel model)
		{
			if (!model.ResolutionDetailId.HasValue || model.ResolutionDetailId == Guid.Empty)
			{
				throw new ApplicationException("Invalid resolution detail Id.");
			}

			if (!model.Resolved.HasValue)
			{
				throw new ApplicationException("Invalid resolved datetime.");
			}

			var issue = await _appDbContext.GetIssueAsync(id);
			if (issue == null)
			{
				throw new ApplicationException("Issue id is invalid.");
			}

			if (issue.ResolutionDetailId != null)
			{
				throw new ApplicationException("Issue has been resolved.");
			}

			_mapper.Map(model, issue);

			_appDbContext.Issues.Update(issue);
			await _appDbContext.SaveChangesAsync();

			await _pubSubService.Publish(new IssueResolvedEvent { Id = issue.Id });
		}

		public async Task<Guid> AddAsync(CreateIssueModel model)
		{
			if (model.CustomerId == Guid.Empty)
			{
				throw new ApplicationException("Invalid customer Id.");
			}

			if (model.ServiceLocationId == Guid.Empty)
			{
				throw new ApplicationException("Invalid service location Id.");
			}

			var issue = _mapper.Map<Issue>(model);

			issue.Id = Guid.NewGuid();
			issue.Created = DateTime.Now;
			issue.Active = true;

			await _appDbContext.Issues.AddAsync(issue);
			await _appDbContext.SaveChangesAsync();

			await _pubSubService.Publish(new IssueCreatedEvent { Id = issue.Id });

			return issue.Id;
		}
	}
}
