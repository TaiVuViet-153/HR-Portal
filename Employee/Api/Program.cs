using Employee.Application;
using Employee.Infrastructure;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<UserDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add Controller
builder.Services.AddControllers();

// Add Dependency Injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Swagger
builder.Services.AddSwaggerGen();


// Setup CORS Policy
var allowedOrigin = new[]
{
    "http://localhost:5127",
    "https://localhost:5127"
};
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", builder =>
    {
        builder.WithOrigins(allowedOrigin)// cho phép bất kỳ domain nào
               .AllowAnyHeader() // header
               .AllowAnyMethod() //method
               .AllowCredentials();
    });
});

// Add Controller
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAny");

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();


app.Run();