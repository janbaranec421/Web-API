namespace Notino.Interfaces
{
    public interface ICacheService
    {
        public Task<T> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration, TimeSpan? slidingExpiration);
        public Task RemoveAsync(string key);
        public Task<bool> ExistsAsync(string key);
    }
}
