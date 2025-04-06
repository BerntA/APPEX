using database;
using lib.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

static void RegisterDbContext<T>(IServiceCollection services, bool isDevBuild, string database) where T : DbContext
{
    services.AddDbContextFactory<T>(options =>
    {
        options.UseSqlite(database);
        options.EnableSensitiveDataLogging(isDevBuild);
        options.EnableDetailedErrors(isDevBuild);
    }, ServiceLifetime.Scoped);
}

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
var isDevBuild = builder.Environment.IsDevelopment();

services.AddControllers();
services.AddHttpClient();
services.AddHttpContextAccessor();
services.AddMemoryCache();

services
    .AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.IncludeFields = true;
    });

RegisterDbContext<ApplicationDbContext>(
    services, isDevBuild, config["AppDatabase"]
);

services.AddHttpClient<BroennoeysundApiService>(options =>
{
    options.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
});

services.AddScoped<CustomerService>();

#if !DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:8080");
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseHsts();
}

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseAntiforgery();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();