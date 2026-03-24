using ae_reporting.api.Data;
using ae_reporting.api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" };
        policy.WithOrigins(allowedOrigins) 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add PostgreSQL Database Context
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

// API Endpoints

// GET Patient by ID
app.MapGet("/api/patients/{patientId}", async (string patientId, AppDbContext db) =>
{
    // Using try/catch here for convenience
    // In a real app I would replace this with global exception handling middleware
    try
    {
        var patient = await db.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        if (patient == null) return Results.NotFound();
        
        return Results.Ok(new { fullName = $"{patient.FirstName} {patient.LastName}" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Query Error: {ex.Message}");
        return Results.Problem("An error occurred while fetching the patient data.");
    }
});

// POST Adverse Event
app.MapPost("/api/adverse-events", async (AdverseEvent ae, AppDbContext db) =>
{
    if (string.IsNullOrEmpty(ae.PatientId) || string.IsNullOrEmpty(ae.Description) || 
        string.IsNullOrEmpty(ae.Severity) || string.IsNullOrEmpty(ae.RelationshipToStudyDrug))
    {
        return Results.BadRequest("All fields are required.");
    }

    ae.DateOfOnset = DateTime.SpecifyKind(ae.DateOfOnset, DateTimeKind.Utc);

    if (ae.DateOfOnset > DateTime.UtcNow)
    {
        return Results.BadRequest("Date of onset cannot be in the future.");
    }

    // Using try/catch here for convenience
    // In a real app I would replace this with global exception handling middleware
    try 
    {
        db.AdverseEvents.Add(ae);
        await db.SaveChangesAsync();
        
        return Results.Created($"/api/adverse-events/{ae.Id}", ae);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Save Error: {ex.Message}");
        return Results.Problem("An error occurred while saving the event.");
    }
});

app.Run();
