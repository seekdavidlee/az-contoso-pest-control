using AutoMapper;
using ContosoPestControl.Issue.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace ContosoPestControl.Tests
{
	[TestClass]
	public class IssueServiceTests
	{
		private IIssueService _issueService;
		private readonly IAppDbContext _appDbContext = Substitute.For<IAppDbContext>();
		public IssueServiceTests()
		{
			_issueService = new IssueService(_appDbContext, Substitute.For<IMapper>(), Substitute.For<IPubSubService>());
		}

		[TestMethod]
		public async Task AddAsync_CustomerIdCannotBeEmptyShouldThrowApplicationException()
		{
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.AddAsync(new CreateIssueModel()));
			Assert.IsTrue(e.Message.ToLower().Contains("customer id"));
		}

		[TestMethod]
		public async Task AddAsync_ServiceLocationIdCannotBeEmptyShouldThrowApplicationException()
		{
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.AddAsync(new CreateIssueModel { CustomerId = Guid.NewGuid() }));
			Assert.IsTrue(e.Message.ToLower().Contains("service location id"));
		}

		[TestMethod]
		public async Task ResolveAsync_MissingResolutionDetailIdShouldThrowApplicationException()
		{
			var guid = Guid.NewGuid();
			_appDbContext.GetIssueAsync(Arg.Is(guid)).Returns(ctx => Task.FromResult<Issue.Core.Issue?>(null));
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.ResolveAsync(guid,
				new ResolveIssueModel { Resolved = DateTime.Today }));
			Assert.IsTrue(e.Message.ToLower().Contains("resolution detail id"));
		}

		[TestMethod]
		public async Task ResolveAsync_EmptyResolutionDetailIdShouldThrowApplicationException()
		{
			var guid = Guid.NewGuid();
			_appDbContext.GetIssueAsync(Arg.Is(guid)).Returns(ctx => Task.FromResult<Issue.Core.Issue?>(null));
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.ResolveAsync(guid,
				new ResolveIssueModel { ResolutionDetailId = Guid.Empty, Resolved = DateTime.Today }));
			Assert.IsTrue(e.Message.ToLower().Contains("resolution detail id"));
		}

		[TestMethod]
		public async Task ResolveAsync_MissingResolvedDateShouldThrowApplicationException()
		{
			var guid = Guid.NewGuid();
			_appDbContext.GetIssueAsync(Arg.Is(guid)).Returns(ctx => Task.FromResult<Issue.Core.Issue?>(null));
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.ResolveAsync(guid,
				new ResolveIssueModel { ResolutionDetailId = Guid.NewGuid() }));
			Assert.IsTrue(e.Message.ToLower().Contains("resolved datetime"));
		}

		[TestMethod]
		public async Task ResolveAsync_InvalidIssueIdShouldThrowApplicationException()
		{
			var guid = Guid.NewGuid();
			_appDbContext.GetIssueAsync(Arg.Is(guid)).Returns(ctx => Task.FromResult<Issue.Core.Issue?>(null));
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.ResolveAsync(guid,
				new ResolveIssueModel { ResolutionDetailId = Guid.NewGuid(), Resolved = DateTime.Today }));
			Assert.IsTrue(e.Message.ToLower().Contains("issue id"));
		}

		[TestMethod]
		public async Task ResolveAsync_IssueAlreadyResolvedShouldThrowApplicationException()
		{
			var guid = Guid.NewGuid();
			_appDbContext.GetIssueAsync(Arg.Is(guid)).Returns(ctx => Task.FromResult<Issue.Core.Issue?>(new Issue.Core.Issue { ResolutionDetailId = Guid.NewGuid() }));
			var e = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _issueService.ResolveAsync(guid,
				new ResolveIssueModel { ResolutionDetailId = Guid.NewGuid(), Resolved = DateTime.Today }));
			Assert.IsTrue(e.Message.ToLower().Contains("been resolved"));
		}
	}
}