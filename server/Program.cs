using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.Server.Services;
using ReleaseMonkey.src.Repositories;

namespace ReleaseMonkey.Server
{
  public class Program
  {
    private static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      {
        builder.Services.AddSingleton<Db>();
        builder.Services.AddSingleton<ProjectsRepository>();
        builder.Services.AddSingleton<UserProjectsRepository>();

        builder.Services.AddSingleton<ProjectsService>();

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
          // For PascalCase naming for returned object properties.
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
      }

      var app = builder.Build();
      {
        if (app.Environment.IsDevelopment())
        {
          app.UseSwagger();
          app.UseSwaggerUI();
        }

        app.MapControllers();
        app.Run();
      }
    }
  }
}