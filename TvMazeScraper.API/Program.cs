using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Application.Services;
using TvMazeScraper.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");
builder.Services.AddDbContext<TvShowDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<TvShowRepository>();
builder.Services.AddScoped<CastMemberRepository>();

// Scraper service
builder.Services.AddHttpClient<TvMazeScraperService>(client =>
{
    client.BaseAddress = new Uri("https://api.tvmaze.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Ensure the database has been created
    var dbContext = scope.ServiceProvider.GetService<TvShowDbContext>();
    dbContext.Database.EnsureCreated();

    // run the scraper before starting the application
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
