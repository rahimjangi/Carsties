using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Config;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
// builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
// {
//     var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//     return new MongoClient(settings.ConnectionString);
// });


builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

// await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));

// await DB.Index<Item>()
// .Key(x => x.Make, KeyType.Text)
// .Key(x => x.Model, KeyType.Text)
// .Key(x => x.Color, KeyType.Text)
// .CreateAsync();

// Initialize MongoDB
await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));

// Create indexes
await DB.Index<Item>()
    .Key(x => x.Make, KeyType.Text)
    .Key(x => x.Model, KeyType.Text)
    .Key(x => x.Color, KeyType.Text)
    .CreateAsync();

app.Run();
