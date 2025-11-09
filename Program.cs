using Supabase;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Leer configuración
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

// 2️⃣ Registrar el cliente Supabase como Singleton (mejor que Scoped)
builder.Services.AddSingleton<Client>(sp =>
{
    var client = new Client(supabaseUrl!, supabaseKey!, new SupabaseOptions
    {
        AutoConnectRealtime = false,
        AutoRefreshToken = true
    });

    // Inicializa el cliente al iniciar el servidor
    client.InitializeAsync().Wait();

    return client;
});

// 3️⃣ Agregar controladores
builder.Services.AddControllers();

// 4️⃣ Construir la app
var app = builder.Build();

app.MapControllers();

app.Run();
