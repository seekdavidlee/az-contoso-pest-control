namespace ContosoPestControl.Issue.Core
{
	public class IssueCreatedEvent : IssueEvent
	{
		public IssueCreatedEvent() : base("IssueCreatedEvent")
		{

		}
		public Guid Id { get; set; }
	}
}
