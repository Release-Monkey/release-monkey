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
            else if (ExecutablePath.Contains("rel-monkey"))
            {
                return Build.Production;
            }
            else
            {
                return Build.Beta;
            }
        }

        public static string ApiUrl
        {
            get
            {
                return GetBuild() switch
                {
                    Build.Developer => "http://localhost:3000",
                    Build.Production => "prod-url",
                    _ => "qa-url"
                };

            }
        }
    }
}