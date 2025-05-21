namespace TracePlayer.DB.Repositories.Geo
{
    public interface IGeoRepository
    {
        Task<string?> GetCountryCode(long ipLong);
    }
}
