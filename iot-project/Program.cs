using iot_project.Data;
using iot_project.Helpers;
using iot_project.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
    }
);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IIdentityCardRepository, IdentityCardRepository>();
builder.Services.AddScoped<ICheckCardHistoryRepository, CheckCardHistoryRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddSingleton<MqttService>(sp =>
{
    var mqttConfig = builder.Configuration.GetSection("Mqtt");
    return new MqttService(
        mqttConfig["Server"] ?? throw new InvalidOperationException("Mqtt:Server is not configured"),
        int.Parse(mqttConfig["Port"] ?? throw new InvalidOperationException("Mqtt:Port is not configured")),
        mqttConfig["Username"] ?? throw new InvalidOperationException("Mqtt:Username is not configured"),
        mqttConfig["Password"] ?? throw new InvalidOperationException("Mqtt:Password is not configured")
    );
});
builder.Services.AddHostedService<MqttSubcriberService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();

app.Run();
