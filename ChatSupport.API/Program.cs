using ChatSupport.Application.Interfaces;
using ChatSupport.Application.Services;
using ChatSupport.Domain.Models;
using ChatSupport.Infrastructure.Background;
using ChatSupport.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Setup Teams and Office Hour
// ------------------------------
var primaryTeams = new List<Team>
{
    //StaticTeams.TeamA,
    StaticTeams.TeamB
    //StaticTeams.TeamC
};

var overflowTeam = StaticTeams.Overflow;
var isOfficeHours = true;

// ------------------------------
// Register Domain Services
// ------------------------------
builder.Services.AddSingleton<List<Team>>(primaryTeams);
builder.Services.AddSingleton<Team>(overflowTeam);
builder.Services.AddSingleton<IAssignmentService, AssignmentService>();
builder.Services.AddSingleton<IQueueService, QueueService>();


// ------------------------------
// Hosted Background Worker
// ------------------------------
builder.Services.AddHostedService<PollingMonitorService>();

// ------------------------------
// Controllers + Swagger
// ------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------
// Middleware
// ------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
