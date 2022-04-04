using ContosoPestControl.Issue.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPestControl.Issue.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class IssueController : ControllerBase
	{
		private readonly ILogger<IssueController> _logger;
		private readonly IIssueService _issueService;

		public IssueController(ILogger<IssueController> logger, IIssueService issueService)
		{
			_logger = logger;
			_issueService = issueService;
		}

		[HttpGet("{issueId}", Name = "GetIssue")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetIssueModel))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Get(Guid issueId)
		{
			var model = await _issueService.GetAsync(issueId);

			if (model == null)
			{
				_logger.LogError($"Issue Id {issueId} is invaild.");
				return NotFound("Issue Id is invaild.");
			}

			return Ok(model);
		}

		[HttpPost(Name = "CreateIssue")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateIssueModelResponse))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Add(CreateIssueModel model)
		{
			try
			{
				var id = await _issueService.AddAsync(model);
				return Ok(new CreateIssueModelResponse { Id = id });
			}
			catch (ApplicationException e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpPut("{issueId}", Name = "UpdateIssue")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Update([FromRoute] Guid issueId, [FromBody] UpdateIssueModel model)
		{
			try
			{
				await _issueService.UpdateAsync(issueId, model);
				return Ok();
			}
			catch (ApplicationException e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpPut("{issueId}/resolution", Name = "ResolveIssue")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Resolve([FromRoute] Guid issueId, [FromBody] ResolveIssueModel model)
		{
			try
			{
				await _issueService.ResolveAsync(issueId, model);
				return Ok();
			}
			catch (ApplicationException e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpDelete("{issueId}", Name = "DeleteIssue")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(Guid issueId)
		{
			try
			{
				await _issueService.DeleteAsync(issueId);
				return Ok();
			}
			catch (ApplicationException e)
			{
				return BadRequest(e.Message);
			}
		}
	}
}