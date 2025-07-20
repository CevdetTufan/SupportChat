using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SupportChat.Api.Endpoints;
using SupportChat.Api.Middlewares;
using SupportChat.Application.Modules;
using SupportChat.Infrastructure.Data;
using SupportChat.Infrastructure.Messaging;
using SupportChat.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(
	builder.Configuration.GetSection("RabbitMQ"));

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseNpgsql(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		npgsql => npgsql.MigrationsAssembly("SupportChat.Infrastructure")));
}

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(container =>
			{
				container.RegisterModule(new InfrastructureModule());
				container.RegisterModule(new ApplicationModule());
			});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
	DbInitializer.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScalarApiReference(options =>
{
	options
		.WithTitle("Support Chat API")
		.WithDarkMode()
		.WithTheme(ScalarTheme.Saturn);
});

app.UseHttpsRedirection();

app.MapChatEndpoints();

await app.RunAsync();