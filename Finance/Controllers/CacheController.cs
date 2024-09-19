using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Serilog;

namespace Finance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public CacheController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // Redis'e veri keyleme
        [HttpPost("set")]
        public async Task<IActionResult> SetCache(string key, string value)
        {
            var db = _redis.GetDatabase();
            TimeSpan expiryTime = TimeSpan.FromMinutes(60); // 60 dakika sonra cache otomatik olarak silinecek
            try
            {
                await db.StringSetAsync(key, value, expiryTime);
                Log.Information("Cache set for key: {Key} with value: {Value}", key, value); // Loglama
                return Ok("Value set in Redis");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting cache for key: {Key}", key); // Hata loglama
                return StatusCode(500, "Error setting cache");
            }
        }

        // Redis'ten veri alma
        [HttpGet("get")]
        public async Task<IActionResult> GetCache(string key)
        {
            var db = _redis.GetDatabase();
            try
            {
                var value = await db.StringGetAsync(key);
                if (!value.HasValue)
                {
                    Log.Warning("Cache key not found: {Key}", key); // Uyarı loglama
                    return NotFound("Key not found");
                }
                Log.Information("Cache retrieved for key: {Key} with value: {Value}", key, value.ToString()); // Başarı loglama
                return Ok(value.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving cache for key: {Key}", key); // Hata loglama
                return StatusCode(500, "Error retrieving cache");
            }
        }
    }
}
