using System.Text.Json;
using Blazored.LocalStorage;
using ReleaseMonkeyWeb.Models;
using Microsoft.JSInterop;

namespace ReleaseMonkeyWeb.Services
{
    public class LocalPreferencesServices
    {
        private readonly ILocalStorageService localStorage;

        public LocalPreferencesServices (ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task SetUser(User user)
        {
            var userString = JsonSerializer.Serialize(user);
            await localStorage.SetItemAsync("USER", userString);
        }

        public async Task<User?> GetUser()
        {
            var userString = await localStorage.GetItemAsync<string>("USER");
            if (userString == null)
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<User>(userString);
            }
        }
    }
}