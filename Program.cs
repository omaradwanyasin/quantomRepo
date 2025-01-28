using backend_api;
using backend_api.Models;
using backend_api.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Register MongoDbSettings from Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("DBSetting"));

// 2️⃣ Register MongoClient as a Singleton
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// 3️⃣ Register MongoDbService as Scoped
builder.Services.AddScoped<MongoDbService>();

// 4️⃣ Register SignalR (If you use SignalR)


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5️⃣ **Make sure SignalR is correctly mapped**
app.MapHub<UpdateHub>("/updateHub");

// 6️⃣ **Ensure MongoDbService is retrieved correctly**
using (var scope = app.Services.CreateScope())
{
    var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    Task.Run(() => mongoDbService.WatchCollection(CancellationToken.None));
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();
app.Run();
