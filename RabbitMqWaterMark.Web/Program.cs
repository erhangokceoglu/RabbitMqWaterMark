using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqWaterMark.Web.BackgroundServices;
using RabbitMqWaterMark.Web.Contexts;
using RabbitMqWaterMark.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "RabbitMqWaterMarkDb");
});

builder.Services.AddSingleton(x => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMq")!
    ),
    DispatchConsumersAsync = true
});

builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<ImageWaterMarkProcessBackgroundService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
