using ChatSupport.Application.Interfaces;
using ChatSupport.Application.Services;
using ChatSupport.Domain.Models;
using ChatSupport.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Setup teams
var teamA = StaticTeams.TeamA;
var teamB = StaticTeams.TeamB;
var teamC = StaticTeams.TeamC;
var overflowTeam = StaticTeams.Overflow;

// Business rule: define office hours
var isOfficeHours = true;

// Initialize services manually
var primaryTeams = new List<Team> { teamA, teamB, teamC };
var assignmentService = new AssignmentService(primaryTeams, overflowTeam);
var queueService = new QueueService(primaryTeams, overflowTeam, isOfficeHours, assignmentService);

// Register as singletons
builder.Services.AddSingleton<IAssignmentService>(assignmentService);
builder.Services.AddSingleton<IQueueService>(queueService);

// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
