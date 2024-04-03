namespace ReleaseMonkey.Server.Models
{
    public record User(int Id, string Name, string Email);

    public record UserWithToken(int Id, string Name, String Email, string Token): User(Id, Name, Email);
}
