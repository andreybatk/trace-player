namespace TracePlayer.DB.Repositories.Api
{
    public interface IApiKeyRepository
    {
        Task<bool> AddKeyHash(string serverIp, string keyHash);
        Task<bool> IsValidKeyHash(string keyHash);
        Task<bool> IsValidServerIp(string serverIp);
        Task<List<string>> GetAllServerIps();
        Task<bool> DeleteApiKeyByServerIp(string serverIp);
    }
}
