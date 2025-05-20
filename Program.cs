using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models.DTOs;
using TravelLogger.Models;

//using TravelLogger.Models

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<TravelLoggerDbContext>(builder.Configuration["TravelLoggerDbConnectionString"]);

var app = builder.Build();

// Comment out HTTPS redirection for now to simplify testing
// app.UseHttpsRedirection();

// Configure Swagger for all environments during development
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

// Add all endpoints here

app.MapPost("/api/logs", (TravelLoggerDbContext db, LogDTO newLogDTO) =>
{
    try
    {
        Log newLog = new Log
        {
            Id = newLogDTO.Id,
            UserId = newLogDTO.UserId,
            CityId = newLogDTO.CityId,
            LoggedTime = DateTime.Now
        };

        db.Logs.Add(newLog);
        db.SaveChanges();

        Log DTOInfo = db.Logs.Include(l => l.User).Include(l => l.City).SingleOrDefault(l => l.Id == newLog.Id);

        return Results.Created($"/api/logs/{newLog.Id}", new LogDTO
        {
            Id = DTOInfo.Id,
            UserId = DTOInfo.UserId,
            CityId = DTOInfo.CityId,
            LoggedTime = DTOInfo.LoggedTime,
            User = DTOInfo.User != null ? new UserDTO
            {
                Id = DTOInfo.User.Id,
                Name = DTOInfo.User.Name,
                Email = DTOInfo.User.Email
            } : null,
            City = DTOInfo.City != null ? new CityDTO
            {
                Id = DTOInfo.City.Id,
                Name = DTOInfo.City.Name,
            } : null
        });
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid Data");
    }
});



app.Run();