using ContosoPestControl.Issue.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

const string apiVersion = "v1";

builder.Services.AddSwaggerGen(option =>
{
	option.SwaggerDoc(apiVersion, new OpenApiInfo { Title = "Issue Service API", Version = apiVersion });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

builder.Services.AddAutoMapper(cfg =>
{
	cfg.CreateMap<CreateIssueModel, Issue>();
	cfg.CreateMap<UpdateIssueModel, Issue>();
	cfg.CreateMap<ResolveIssueModel, Issue>();
	cfg.CreateMap<Issue, GetIssueModel>();
});

builder.Services.AddTransient<IIssueService, IssueService>();
builder.Services.AddTransient<IAppDbContext, AppDbContext>();

var appId = builder.Configuration["APP_ID"];

if (string.IsNullOrEmpty(appId))
{
	builder.Services.AddTransient<IPubSubService, PassThruPubSubService>();
}
else
{
	builder.Services.AddTransient<IPubSubService, DaprClientPubSubService>();
	builder.Services.AddDaprClient();
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
	var connectionString = $"Data Source={builder.Configuration["DbServer"]};Initial Catalog={builder.Configuration["DbName"]}; User Id={builder.Configuration["DbUsername"]};Password={builder.Configuration["DbPassword"]}";
	options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
		 {
			 sqlOptions.EnableRetryOnFailure();
		 });
});

builder.Services.AddHealthChecks()
	.AddCheck<AppDbContext>("Database");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use with UseEndpoints
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
	endpoints.MapHealthChecks("/health").AllowAnonymous();
});

app.Run();
