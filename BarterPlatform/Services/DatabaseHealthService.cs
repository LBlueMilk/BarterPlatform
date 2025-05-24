using BarterPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BarterPlatform.Services
{
    public class DatabaseHealthService : IDatabaseHealthService
    {
        private readonly BarterPlatformContext _context;
        private readonly ILogger<DatabaseHealthService> _logger;

        public DatabaseHealthService(BarterPlatformContext context, ILogger<DatabaseHealthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                // 可根據您資料表改用最輕量的查詢
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "資料庫連線檢查失敗");
                return false;
            }
        }
    }

}
