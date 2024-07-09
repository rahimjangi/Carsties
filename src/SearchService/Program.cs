using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Config;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
// builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
// {
//     var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//     return new MongoClient(settings.ConnectionString);
// });

builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{

    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception ex)
    {

        Console.WriteLine(ex);
    }
});



app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(2));
