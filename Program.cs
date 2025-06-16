using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PokemonReviewApp.Data;
using PokemonReviewApp;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Repository;
using System.Text.Json.Serialization;
using System.Collections;




var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new Exception("Connection string is missing!");

Console.WriteLine($"Using DB: {connectionString}");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure()
    ));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

Console.WriteLine("=== ENV VARIABLES ===");
foreach (var envVar in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    Console.WriteLine($"{envVar.Key}: {envVar.Value}");
}
Console.WriteLine("====================");
Console.WriteLine("CONNECTION STRING: " +
    builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokemon API v1");
});


//app.UseHttpsRedirection();
app.UseExceptionHandler("/error");
app.MapGet("/error", () => "Ïðîèçîøëà îøèáêà. Ïðîâåðüòå ëîãè ñåðâåðà.");


app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");
app.MapGet("/", () => "Pokemon API is working!");
app.MapGet("/test", () => "Test endpoint");
app.MapGet("/health", () => "Healthy");
app.MapGet("/db-debug", (DataContext context) =>
{
    try
    {
        var canConnect = context.Database.CanConnect();
        return $"DB Connection: {canConnect}";
    }
    catch (Exception ex)
    {
        return $"DB Error: {ex.Message}";
    }
});
app.MapGet("/test-db", async (DataContext context) =>
{
    return await context.Pokemon.AnyAsync()
        ? "DB connection OK"
        : "DB connected but empty";
});
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

app.Run("http://*:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));