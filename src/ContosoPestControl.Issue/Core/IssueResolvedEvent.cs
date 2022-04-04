namespace ContosoPestControl.Issue.Core
{
	public partial class IssueService
	{
		public class IssueResolvedEvent : IssueEvent
		{
			public IssueResolvedEvent() : base("IssueResolvedEvent")
			{

			}
			public Guid Id { get; set; }
		}
	}
}
