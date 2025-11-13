using censudex_inventory_service.src.Interface;
using censudex_inventory_service.src.Service;
using MassTransit;
using Supabase;

var builder = WebApplication.CreateBuilder(args);


var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

builder.Services.AddSingleton<Client>(sp =>
{
    var client = new Client(supabaseUrl!, supabaseKey!, new SupabaseOptions
    {
        AutoConnectRealtime = false,
        AutoRefreshToken = true
    });


    client.InitializeAsync().Wait();

    return client;
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
