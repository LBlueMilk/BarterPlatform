namespace BarterPlatform.Services
{
    public interface IDatabaseHealthService
    {
        Task<bool> CheckConnectionAsync();
    }
}
