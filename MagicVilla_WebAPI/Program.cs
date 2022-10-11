//using Serilog;

using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// para q la app sepa q se tiene q conectar a la BD a traves de EF y para agarrar el connection string con la config
// el hecho de registrarlo ( con builder.Services. ... ) aca es lo que me permite inyectarlo en los componentes
var connectionString = builder.Configuration.GetConnectionString("DefaultSQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Add services to the container.

// para logear al archivo ( video 32 )
// Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//     .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
// builder.Host.UseSerilog();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// para poder inyectar mi loger
//builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
