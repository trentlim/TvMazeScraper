using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Application.Services;
using TvMazeScraper.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<TvShowDbContext>(opt =>
    opt.UseInMemoryDatabase("SubscriptionAPI"));
builder.Services.AddScoped<TvShowRepository>();

// Scraper service
builder.Services.AddHttpClient<TvMazeScraperService>(client =>
{
    client.BaseAddress = new Uri("https://api.tvmaze.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Run the scraper before starting the application
using (var scope = app.Services.CreateScope())
{
    var scraperService = scope.ServiceProvider.GetRequiredService<TvMazeScraperService>();
    await scraperService.RunScraperAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
