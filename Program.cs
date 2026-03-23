using ae_reporting.api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Add CORS (so Vue can call this API locally)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        // Vite defaults to localhost:5173
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Add PostgreSQL Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueFrontend");

// 3. Create our "Hello World" endpoint!
app.MapGet("/api/hello", async (AppDbContext db) =>
{
    // Try to get a message, or return a default one if the table does not exist yet
    try
    {
        var message = await db.HelloMessages.FirstOrDefaultAsync();
        return Results.Ok(message ?? new ae_reporting.api.Models.HelloMessage { Id = 0, Text = "Hello from PostgreSQL + .NET 8!" });
    }
    catch
    {
        return Results.Ok(new ae_reporting.api.Models.HelloMessage { Id = -1, Text = "Cannot connect to database or tables are not created yet!" });
    }
});

app.Run();
