using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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
		options.UseInMemoryDatabase("SupportChatDb"));
}

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(container =>
			{
				container.RegisterModule(new InfrastructureModule());
				container.RegisterModule(new ApplicationModule());
			});

builder.Services.AddOpenApi();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider
	.GetRequiredService<AppDbContext>()
	.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();