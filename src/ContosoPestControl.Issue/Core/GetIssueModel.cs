namespace ContosoPestControl.Issue.Core
{
	public class GetIssueModel
	{
		public Guid Id { get; internal set; }
		public Guid CustomerId { get; internal set; }
		public Guid ServiceLocationId { get; internal set; }
		public DateTime Created { get; internal set; }
		public string? Subject { get; internal set; }
		public string? Details { get; internal set; }
		public DateTime? Resolved { get; internal set; }
		public Guid? ResolutionDetailId { get; set; }
	}
}