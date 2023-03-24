using Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // This creates server

// Add services to the container.//Services

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Db Context Service
builder.Services.AddDbContext<DataContext>(opt =>{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.//Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //Swagger is middleware
    app.UseSwaggerUI();
}




app.MapControllers();//This middleware tells app where to send request on the basis of controllers


//To add Database
using var scope = app.Services.CreateScope(); //using keyword will destory the variables memory when it completes executing
var services = scope.ServiceProvider;

//Create Database
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    //seed default data into database
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex,"An error occured during migration");
    throw;
}

app.Run();
