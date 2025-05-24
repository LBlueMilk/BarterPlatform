using BarterPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarterPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IDatabaseHealthService _databaseHealthService;

        public PingController(IDatabaseHealthService databaseHealthService)
        {
            _databaseHealthService = databaseHealthService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var isReady = await _databaseHealthService.CheckConnectionAsync();
            return isReady ? Ok("Database is ready") : StatusCode(503, "Database not ready");
        }
    }
}
