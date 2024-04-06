using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using cli.models;

namespace cli.services
{
    internal class LocalPreferencesServices
    {

        public LocalPreferencesServices()
        {
            LoadCache();
        }

        private Dictionary<string, object> cache = [];

        public void SetUser(User user)
        {
            cache["User"] = JsonSerializer.Serialize(user);
            SaveCache();
        }

        public User? GetUser()
        {
            if (cache.TryGetValue("User", out object? value))
            {
                return JsonSerializer.Deserialize<User>(value.ToString()!);
            }
            {
                return null;
            }
        }

        public void ClearUser()
        {
            cache.Remove("User");
            SaveCache();
        }

        public void SetProject(Project project)
        {
            cache["Project"] = JsonSerializer.Serialize(project);
            SaveCache();            
        }

        public Project? GetProject()
        {
            if (cache.TryGetValue("Project", out object? value))
            {
                return JsonSerializer.Deserialize<Project>(value.ToString()!);
            }
            {
                return null;
            }
        }

        public void ClearProject()
        {
            cache.Remove("Project");
            SaveCache();
        }        

        private void SaveCache()
        {
            SaveToPreferencesFile(JsonSerializer.Serialize(cache));
        }

        private void LoadCache()
        {
            cache = JsonSerializer.Deserialize<Dictionary<string, object>>(GetPreferencesFile())!;
        }

        private static void SaveToPreferencesFile(string contents)
        {
            File.WriteAllText(PreferencesFilePath, contents);
        }

        private static string GetPreferencesFile()
        {
            try
            {
                using var settingsFile = new StreamReader(PreferencesFilePath);
                return settingsFile.ReadToEnd();
            }
            catch (FileNotFoundException)
            {
                return "{}";
            }
        }

        private static string PreferencesFilePath
        {
            get => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "settings.json");
        }
    }
}
