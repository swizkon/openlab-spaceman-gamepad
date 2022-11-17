using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;


services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Game API", Version = "v1" });
    
    c.CustomSchemaIds(type => type.ToString());
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

services.AddSignalR();

services.AddCors(options => {
    options.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyHeader();
        //b.AllowAnyOrigin();
        b.AllowAnyMethod();
        b.SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<DebugHub>("/hubs/debugHub");
});


app.Run();


public class DebugHub : Hub<IDebugHubClient>
{
    public async Task Broadcast(string message)
    {
        await Clients.All.Broadcast(message);
    }
}


public interface IDebugHubClient
{
    Task Broadcast(string message);
}