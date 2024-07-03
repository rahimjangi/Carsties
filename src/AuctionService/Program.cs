using AuctionService.Data;
using AuctionService.RequestHellpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingProfiles));

var app = builder.Build();



app.UseAuthorization();

app.MapControllers();
try
{
    DbInitializer.InitDB(app);
}
catch (Exception e)
{

    Console.WriteLine(e);
}

app.Run();
