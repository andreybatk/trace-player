namespace TracePlayer.DB.Repositories.Users
{
    public interface IUserRepository
    {
        Task<string?> GetSteamId(Guid id);
    }
}
