using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.Server.Services;
using ReleaseMonkey.src.Repositories;
using ReleaseMonkey.Server.Models;
using System.Text;

namespace ReleaseMonkey.Server
{
  public class Program
  {
    private static void Main(string[] args)
    {
      DotNetEnv.Env.Load();

      var githubService = new GithubService(
          Environment.GetEnvironmentVariable("GITHUB_APP_CLIENT_ID")!,
          Environment.GetEnvironmentVariable("GITHUB_APP_CLIENT_SECRET")!);
      var db = new Db();
      var usersRepository = new UsersRepository(db);

      var builder = WebApplication.CreateBuilder(args);
      {
        builder.Services.AddSingleton(db);
        builder.Services.AddSingleton(usersRepository);
        builder.Services.AddSingleton<ProjectsRepository>();
        builder.Services.AddSingleton<UserProjectsRepository>();
        builder.Services.AddSingleton<ReleasesRepository>();
        builder.Services.AddSingleton<ReleaseTestersRepository>();

        // TODO: Read secret from envrinment variable and revoke this one.
        builder.Services.AddSingleton(githubService);

        builder.Services.AddSingleton<UsersService>();
        builder.Services.AddSingleton<ProjectsService>();
        builder.Services.AddSingleton<ReleasesService>();
        builder.Services.AddSingleton<ReleaseTestersService>();
        builder.Services.AddSingleton<UserProjectsService>();

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

        app.Use(async (context, next) =>
        {
          if (context.Request.Method == "POST" && context.Request.Path.ToString().Contains("/users"))
          {
            // Login Request, do not authenticate.
            await next.Invoke();
          }
          else
          {
            if (context.Request.Headers.ContainsKey("authorization"))
            {
              try
              {
                var githubToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var (_, Email) = await githubService.GetUserInfo(githubToken);
                var systemUser = usersRepository.FindByEmail(Email);
                context.Features.Set(new UserWithToken(systemUser.Id, systemUser.Name, systemUser.Email, githubToken));
                await next.Invoke();
              }
              catch (HttpRequestException)
              {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.ContentType = "application/text";
                await context.Response.WriteAsync("Invalid token used. Try logging out and back in again.");
              }
            }
            else
            {
              context.Response.StatusCode = StatusCodes.Status401Unauthorized;
              context.Response.Headers.ContentType = "application/text";
              await context.Response.WriteAsync("No authorization token in request.");
            }
          }
        });

        app.MapControllers();
        app.Run();
      }
    }
  }
}