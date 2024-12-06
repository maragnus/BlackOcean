using BlackOcean.App;
using BlackOcean.Simulation;
using BlackOcean.Simulation.Scenarios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WebSocketServer>();
builder.Services.AddSingleton<Game>(_ => Game.StartScenario<SolarSystemScenario>());
builder.Services.AddHostedService<GameHostedService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});
//app.UseMiddleware<WebSocketMiddleware>(

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapFallbackToFile("index.html");

app.Run();