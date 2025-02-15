#if !SQLiteVersion
// <snippet_sx_all>
// <snippet_sx_filter>
// <snippet_sx>
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Logs dans la console
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Logs dans un fichier journalier
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(); // Remplace le système de log par défaut par Serilog

builder.Services.AddRazorPages();

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContext")));
// </snippet_sx>

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

// Activation de la journalisation des requêtes HTTP avec Serilog
app.UseSerilogRequestLogging();

// <snippet_ensure>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SchoolContext>();

    // Log de la création de la base
    Log.Information("Vérification et initialisation de la base de données...");
    
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}
// </snippet_ensure>

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
// </snippet_sx_all>

#else
// <snippet_sqlite_all>
// <snippet_sqlite_filter>
// <snippet_sqlite>
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SchoolContextSQLite")));
// </snippet_sqlite>

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SchoolContext>();

    Log.Information("Vérification et initialisation de la base de données (SQLite)...");

    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
// </snippet_sqlite_all>
#endif
