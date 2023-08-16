using HotelGetter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HotelGetterString");
builder.Services.AddDbContext<hotelDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<hotelDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowEvery", c =>c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowEvery");
app.MapControllers();

app.Run();
