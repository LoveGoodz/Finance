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

        // Redis'e veri set etme
        [HttpGet("set")]
        public async Task<IActionResult> SetCache(string key, string value)
        {
            var db = _redis.GetDatabase();
            TimeSpan expiryTime = TimeSpan.FromMinutes(60); // 60 dakika sonra cache otomatik olarak silinecek
            try
            {
                await db.StringSetAsync(key, value, expiryTime);  // Redis'e key-value şeklinde veri kaydediyoruz
                Log.Information("Set cache key: {Key} with value: {Value}", key, value);
                return Ok("Value set in Redis");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting cache key: {Key}", key);
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
                var value = await db.StringGetAsync(key); // Redis'teki veriyi key ile okuyoruz
                if (!value.HasValue)
                {
                    Log.Warning("Cache key not found: {Key}", key);
                    return NotFound("Key not found");
                }
                Log.Information("Retrieved cache key: {Key} with value: {Value}", key, value.ToString());
                return Ok(value.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving cache key: {Key}", key);
                return StatusCode(500, "Error retrieving cache");
            }
        }
    }
}
