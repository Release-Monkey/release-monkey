using cli.models;

namespace cli.services
{
    internal class AuthService(LocalPreferencesServices preferencesServices)
    {
        private User user;

        public User User { get => user; }
        public string AuthToken { get; }


        public Task<User> Initialise()
        {
            throw new NotImplementedException();
        }

        public Task<User> LoginWithGithub()
        {
            throw new NotImplementedException();
        }

        public Task<User> Logout() { throw new NotImplementedException(); }


    }
}
