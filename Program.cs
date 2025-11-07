using Supabase;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];


builder.Services.AddScoped<Client>(sp => new Client(supabaseUrl!, supabaseKey!));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
