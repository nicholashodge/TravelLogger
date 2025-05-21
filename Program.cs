using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models.DTOs;
using TravelLogger.Models;
using System.Threading.Tasks.Sources;

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

app.MapPut("/api/logs/{Id}", (TravelLoggerDbContext db, LogDTO newLogDTO, int Id) =>
{
    try
    {
        Log oldLog = db.Logs.SingleOrDefault(l => l.Id == Id);

        if (oldLog == null)
        {
            return Results.NotFound();
        }

        oldLog.UserId = newLogDTO.UserId;
        oldLog.CityId = newLogDTO.CityId;
        oldLog.LoggedTime = newLogDTO.LoggedTime;

        db.SaveChanges();

        Log DTOInfo = db.Logs.Include(l => l.User).Include(l => l.City).SingleOrDefault(l => l.Id == Id);

        return Results.Ok(new LogDTO
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

app.MapGet("/api/users/{userId}/logs", (TravelLoggerDbContext db, int userId) =>
{
    User user = db.Users.Include(u => u.Logs).ThenInclude(u => u.City).SingleOrDefault(u => u.Id == userId);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Logs = user.Logs.Select(log => new LogDTO
            {
                Id = log.Id,
                UserId = log.UserId,
                CityId = log.CityId,
                LoggedTime = log.LoggedTime,
                City = log.City != null ? new CityDTO
                {
                    Id = log.City.Id,
                    Name = log.City.Name
                } : null
            }).ToList()
        }
    );
});

app.MapGet("/api/cities/{cityId}/logs", (TravelLoggerDbContext db, int cityId) =>
{
    City city = db.Cities.Include(c => c.Logs).ThenInclude(c => c.User).SingleOrDefault(c => c.Id == cityId);

    if (city == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        new CityDTO
        {
            Id = city.Id,
            Name = city.Name,
            Logs = city.Logs.Select(log => new LogDTO
            {
                Id = log.Id,
                UserId = log.UserId,
                CityId = log.CityId,
                LoggedTime = log.LoggedTime,
                User = log.User != null ? new UserDTO
                {
                    Id = log.User.Id,
                    Name = log.User.Name,
                    Email = log.User.Email
                } : null
            }).ToList()
        }
    );
});

app.MapDelete("/api/logs/{Id}", (TravelLoggerDbContext db, int Id) =>
{
    Log log = db.Logs.SingleOrDefault(l => l.Id == Id);

    if (log == null)
    {
        return Results.NotFound();
    }

    db.Logs.Remove(log);
    db.SaveChanges();

    return Results.NoContent();
});

app.MapGet("/api/cities", (TravelLoggerDbContext db) =>
{
    return db.Cities.Select(c => new CityDTO
    {
        Id = c.Id,
        Name = c.Name
    }).ToList();
});

app.MapGet("api/cities/{id}", (int id, TravelLoggerDbContext db) =>
{
    bool exists = db.Cities.Any(c => c.Id == id);
    if (!exists)
    {
        return Results.NotFound();
    }
    var city = db.Cities.Include(c=> c.Recommendation).Include(c=> c.Logs).ThenInclude(l=> l.User).Select(c => new CityDTO
    {
        Id = c.Id,
        Name = c.Name,
        Recommendation = new RecommendationDTO
        {
            Id = c.Recommendation.Id,
            CityId = c.Recommendation.CityId,
            Text = c.Recommendation.Text
        },
        Logs = c.Logs.Select(log=> new LogDTO
        {
            Id = log.Id,
            UserId = log.UserId,
            CityId = log.CityId,
            LoggedTime = log.LoggedTime,
            User = log.User != null ? new UserDTO
            {
                Id = log.User.Id,
                Name = log.User.Name,
                Email = log.User.Email
            } : null
        }).ToList()
       
    }).Single(c => c.Id == id);
    return Results.Ok(city);
});

app.MapPost("/api/recommendations", (TravelLoggerDbContext db, RecommendationDTO recommendationDTO) =>
{
    try{
        Recommendation recommendationPost = new Recommendation
        {
            Id = recommendationDTO.Id,
            CityId = recommendationDTO.CityId,
            UserId = recommendationDTO.UserId,
            Text = recommendationDTO.Text
        };

        db.Recommendations.Add(recommendationPost);
        db.SaveChanges();

        Recommendation rDTO = db.Recommendations.Include(r => r.City).Include(r => r.User).SingleOrDefault(r => r.Id == recommendationDTO.Id);

        return Results.Created($"/api/recommendations/{recommendationDTO.Id}", new RecommendationDTO
        {
            Id = rDTO.Id,
            CityId = rDTO.CityId,
            UserId = rDTO.UserId,
            Text = rDTO.Text,
            City = rDTO.City != null ? new CityDTO
            {
                Id = rDTO.City.Id,
                Name = rDTO.City.Name
            } : null,
            User = rDTO.User != null ? new UserDTO
            {
                Id = rDTO.User.Id,
                Name = rDTO.User.Name,
                Email = rDTO.User.Email
            } : null
        });

    } catch (DbUpdateException){
        return Results.BadRequest("Invalid Data");
    }
});

app.MapPut("/api/recommendations/{Id}", (TravelLoggerDbContext db, RecommendationDTO recommendationPut, int Id) =>
{
    try{

        Recommendation outdatedRecommendation = db.Recommendations.SingleOrDefault(r => r.Id == Id);
        if(outdatedRecommendation == null){
            return Results.NotFound();
        }

        //update the current recommendation with the new recommendations info
        outdatedRecommendation.CityId = recommendationPut.CityId;
        outdatedRecommendation.Text = recommendationPut.Text;
        db.SaveChanges();

        Recommendation rDTO = db.Recommendations.Include(r => r.City).Include(r => r.User).SingleOrDefault(r => r.Id == Id);

        return Results.Ok(new RecommendationDTO
        {
            Id = rDTO.Id,
            CityId = rDTO.CityId,
            Text = rDTO.Text,
            City = rDTO.City != null ? new CityDTO {
                Id = rDTO.City.Id,
                Name = rDTO.City.Name
            } : null,
            User = rDTO.User != null ? new UserDTO {
                Id = rDTO.User.Id,
                Name = rDTO.User.Name,
                Email = rDTO.User.Email
            } : null
        });

    } catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid Data");
    }
});

app.MapDelete("/api/recommendations/{Id}", (TravelLoggerDbContext db, int Id) =>
{
    Recommendation rec = db.Recommendations.SingleOrDefault(r => r.Id == Id);

    if(rec == null){
        return Results.NotFound();
    }

    db.Recommendations.Remove(rec);
    db.SaveChanges();

    return Results.NoContent();
});

app.MapGet("/api/recommendations/{Id}", (TravelLoggerDbContext db, int Id) =>
{

    try
    {

        Recommendation rec = db.Recommendations.Include(r => r.City).Include(r => r.User).SingleOrDefault(r => r.Id == Id);

        return Results.Ok(new RecommendationDTO
        {
            Id = rec.Id,
            CityId = rec.CityId,
            UserId = rec.UserId,
            Text = rec.Text,
            City = rec.City != null ? new CityDTO
            {
                Id = rec.City.Id,
                Name = rec.City.Name
            } : null,
            User = rec.User != null ? new UserDTO
            {
                Id = rec.User.Id,
                Name = rec.User.Name,
                Email = rec.User.Email
            } : null
        });


    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid Data");
    }
});

app.MapPost("/api/users", (TravelLoggerDbContext db, UserDTO newUser) =>
{
    try
    {
        db.Users.Add(new User { Email = newUser.Email, Name = newUser.Name });
        db.SaveChanges();

        var createdUser = db.Users.SingleOrDefault(u => u.Email == newUser.Email && u.Name == newUser.Name);

        return Results.Created($"/api/users/{createdUser.Id}", new UserDTO
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email
        });
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid Data");
    }
});

app.MapPut("/api/users/{Id}", (TravelLoggerDbContext db, UserDTO newUser, int Id) =>
{
    try
    {
        User oldUser = db.Users.SingleOrDefault(u => u.Id == Id);

        if (oldUser == null)
        {
            return Results.NotFound();
        }

        oldUser.Name = newUser.Name;
        oldUser.Email = newUser.Email;

        db.SaveChanges();

        return Results.Ok(new UserDTO
        {
            Id = oldUser.Id,
            Name = oldUser.Name,
            Email = oldUser.Email
        });
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest();
    }
});

app.MapGet("/api/cities/{cityId}/users", (TravelLoggerDbContext db, int cityId) =>
{
    City city = db.Cities.Include(c => c.Logs).ThenInclude(c => c.User).SingleOrDefault(c => c.Id == cityId);

    if (city == null)
    {
        return Results.NotFound();
    }

    city.Logs = city.Logs.OrderByDescending(l => l.LoggedTime).DistinctBy(l => l.User.Email).ToList();

    return Results.Ok(new CityDTO
    {
        Id = city.Id,
        Name = city.Name,
        Logs = city.Logs.Select(log => new LogDTO
        {
            Id = log.Id,
            UserId = log.UserId,
            CityId = log.CityId,
            LoggedTime = log.LoggedTime,
            User = log.User != null ? new UserDTO
            {
                Id = log.User.Id,
                Name = log.User.Name,
                Email = log.User.Email
            } : null
        }).ToList()
    });
});

app.MapGet("/api/users/{Id}", (TravelLoggerDbContext db, int Id) =>
{
    User user = db.Users.Include(u => u.Logs).ThenInclude(u => u.City).Include(u => u.Recommendations).ThenInclude(u => u.City).SingleOrDefault(u => u.Id == Id);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new UserDTO
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Logs = user.Logs.Select(log => new LogDTO
        {
            Id = log.Id,
            UserId = log.UserId,
            CityId = log.CityId,
            LoggedTime = log.LoggedTime,
            City = log.City != null ? new CityDTO
            {
                Id = log.City.Id,
                Name = log.City.Name
            } : null
        }).ToList(),
        Recommendations = user.Recommendations.Select(rec => new RecommendationDTO
        {
            Id = rec.Id,
            CityId = rec.CityId,
            UserId = rec.UserId,
            Text = rec.Text,
            City = rec.City != null ? new CityDTO
            {
                Id = rec.City.Id,
                Name = rec.City.Name
            } : null
        }).ToList()
    });
});

app.MapGet("/api/users/signin/{Email}", (TravelLoggerDbContext db, string Email) =>
{
    User user = db.Users.Include(u => u.Logs).ThenInclude(u => u.City).Include(u => u.Recommendations).ThenInclude(u => u.City).SingleOrDefault(u => u.Email == Email);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new UserDTO
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Logs = user.Logs.Select(log => new LogDTO
        {
            Id = log.Id,
            UserId = log.UserId,
            CityId = log.CityId,
            LoggedTime = log.LoggedTime,
            City = log.City != null ? new CityDTO
            {
                Id = log.City.Id,
                Name = log.City.Name
            } : null
        }).ToList(),
        Recommendations = user.Recommendations.Select(rec => new RecommendationDTO
        {
            Id = rec.Id,
            CityId = rec.CityId,
            UserId = rec.UserId,
            Text = rec.Text,
            City = rec.City != null ? new CityDTO
            {
                Id = rec.City.Id,
                Name = rec.City.Name
            } : null
        }).ToList()
    });
});

app.MapPost("/api/upvotes", (TravelLoggerDbContext db, UpvoteDTO upvoteDTO) =>
{
    try {

        Upvote upvotePost = new Upvote
        {
            Id = upvoteDTO.Id,
            UserId = upvoteDTO.UserId,
            RecommendationId = upvoteDTO.RecommendationId
        };

        db.Upvotes.Add(upvotePost);
        db.SaveChanges();

        Upvote uDTO = db.Upvotes.Include(u => u.User).Include(u => u.Recommendation).SingleOrDefault(u => u.Id == upvoteDTO.Id);

        return Results.Created($"/api/upvotes/{upvoteDTO.Id}", new UpvoteDTO
        {
            Id = uDTO.Id,
            UserId = uDTO.UserId,
            RecommendationId = uDTO.RecommendationId,
            User = uDTO.User != null ? new UserDTO
            {
                Id = uDTO.User.Id,
                Name = uDTO.User.Name,
                Email = uDTO.User.Email
            } : null,
            Recommendation = uDTO.Recommendation != null ? new RecommendationDTO
            {
                Id = uDTO.Recommendation.Id,
                CityId = uDTO.Recommendation.CityId,
                UserId = uDTO.Recommendation.UserId,
                Text = uDTO.Recommendation.Text
            } : null
        });

    } catch (DbUpdateException){
        return Results.BadRequest("Invalid Data");
    }
});

app.MapDelete("/api/upvotes/{Id}", (TravelLoggerDbContext db, int Id) =>
{
    Upvote upvote = db.Upvotes.SingleOrDefault(u => u.Id == Id);
    if(upvote == null){
        return Results.NotFound();
    }

    db.Upvotes.Remove(upvote);
    db.SaveChanges();

    return Results.NoContent();

});

app.Run();