using Npgsql;
using webapi.Data;
using webapi.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register UnitOfWork to work as main controller
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// user cors for access control (allow all access)
builder.Services.AddCors();

// configure db connection
var connectionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(m => m.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();