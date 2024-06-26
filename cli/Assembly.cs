using System.Diagnostics;

namespace cli
{
  internal class Assembly
  {
    public enum Build
    {
      Developer,
      Beta,
      Production,
    }

    public static string Version
    {
      get => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "--";
    }

    public static string ExecutablePath
    {
      get => Environment.ProcessPath!;
    }

    public static Build GetBuild()
    {
      if (ExecutablePath.Contains("Debug"))
      {
        return Build.Developer;
      }
      else if (ExecutablePath.Contains("rmkb"))
      {
        return Build.Beta;
      }
      else
      {
        return Build.Production;
      }
    }

    public static string ApiUrl
    {
      get
      {
        return GetBuild() switch
        {
          Build.Developer => "http://localhost:3000",
          Build.Production => "http://52.210.18.60:5000",
          _ => "http://52.210.18.60:3000"
        };

      }
    }
  }
}