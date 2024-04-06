using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;

namespace ReleaseMonkey.Server.Services
{
    public class UsersService(GithubService githubService, UsersRepository usersRepository)
    {
        public async Task<UserWithToken> SignInWithGithubAccessCode(string accessCode)
        {
            var (Name, Email, Token) = await githubService.GetUser(accessCode);
            var dbUser = usersRepository.InsertOrUpdateUser(Name, Email);
            return new UserWithToken(dbUser.Id, dbUser.Name, dbUser.Email, Token);
        }

        public User GetUserById(int userId)
        {
            return usersRepository.GetUserById(userId);
        }

        public async Task<(string Name, string Email)> GetUserInfoByToken(string token)
        {
            var userInfo = await githubService.GetUserInfo(token);
            return (userInfo.Name, userInfo.Email);
        }
    }
}
