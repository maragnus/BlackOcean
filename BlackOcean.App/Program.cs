using BlackOcean.App;
using BlackOcean.Simulation;
using BlackOcean.Simulation.Scenarios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Game>(_ => Game.StartScenario<SolarSystemScenario>());
builder.Services.AddSingleton<GameService>();
builder.Services.AddHostedService<GameHostedService>();
builder.Services.AddSingleton<WebSocketMiddleware>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});

app.UseMiddleware<WebSocketMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapFallbackToFile("index.html");

app.Run();