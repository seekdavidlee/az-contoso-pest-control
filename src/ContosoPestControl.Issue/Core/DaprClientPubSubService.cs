using Dapr.Client;

namespace ContosoPestControl.Issue.Core
{
	public class PassThruPubSubService : IPubSubService
	{
		public Task Publish<T>(T message) where T : IssueEvent
		{
			return Task.CompletedTask;
		}
	}

	public class DaprClientPubSubService : IPubSubService
	{
		private readonly DaprClient _daprClient;
		private readonly IConfiguration _configuration;

		public DaprClientPubSubService(DaprClient daprClient, IConfiguration configuration)
		{
			_daprClient = daprClient;
			_configuration = configuration;
		}

		public async Task Publish<T>(T message) where T : IssueEvent
		{
			await _daprClient.PublishEventAsync(_configuration["PubsubName"], _configuration["PubsubTopic"], message);
		}
	}

	public abstract class IssueEvent
	{
		public IssueEvent(string eventName)
		{
			EventName = eventName;
		}

		public string EventName { get; private set; }
	}
}
