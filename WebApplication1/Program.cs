using EvolveDb;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

try
{
    var cnx = new SqlConnection(builder.Configuration.GetConnectionString("Default"));
    var evolve = new Evolve(cnx, msg => Console.WriteLine(msg))
    {
        EmbeddedResourceAssemblies = new[] { typeof(Program).Assembly },
        EmbeddedResourceFilters = new[] { "WebApplication1.Migrations" },
        //Locations = new[] { "filesystem:./Migrations/Scripts" }, 
        IsEraseDisabled = true,
        MetadataTableName = "_EvolveMigrations"
    };

    evolve.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine("Database migration failed." + ex.Message);
    throw ex;
}

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
