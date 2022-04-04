using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoPestControl.Issue.Core
{
	[Table("issue")]
	public class Issue
	{
		public Guid Id { get; set; }
		public Guid CustomerId { get; set; }
		public Guid ServiceLocationId { get; set; }
		public DateTime Created { get; set; }
		public string? Subject { get; set; }
		public string? Details { get; set; }
		public DateTime? Resolved { get; set; }
		public Guid? ResolutionDetailId { get; set; }
		public bool Active { get; set; }
	}
}