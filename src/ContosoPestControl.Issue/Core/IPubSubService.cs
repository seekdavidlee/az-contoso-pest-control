namespace ContosoPestControl.Issue.Core
{
	public interface IPubSubService
	{
		Task Publish<T>(T item) where T : IssueEvent;
	}
}
