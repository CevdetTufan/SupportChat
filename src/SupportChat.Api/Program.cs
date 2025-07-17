using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SupportChat.Infrastructure.Data;
using SupportChat.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseInMemoryDatabase("SupportChatDb"));
}

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(container =>
			{
				container.RegisterModule(new InfrastructureModule());
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