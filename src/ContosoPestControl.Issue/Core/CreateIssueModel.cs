using System.ComponentModel.DataAnnotations;

namespace ContosoPestControl.Issue.Core
{
	public class CreateIssueModel
	{
		public Guid CustomerId { get; set; }
		public Guid ServiceLocationId { get; set; }

		[Required]
		public string? Subject { get; set; }

		[Required]
		public string? Details { get; set; }
	}
}