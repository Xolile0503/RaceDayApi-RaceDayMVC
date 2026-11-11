using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Services; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IBlobService, LocalBlobService>(); 

builder.Services.AddDbContext<RaceDayContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RaceDayConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();