using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

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
            await db.StringSetAsync(key, value);  // Redis'e key-value şeklinde veri kaydediyoruz
            return Ok("Value set in Redis");
        }

        // Redis'ten veri alma
        [HttpGet("get")]
        public async Task<IActionResult> GetCache(string key)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key); // Redis'teki veriyi key ile okuyoruz
            if (!value.HasValue)
            {
                return NotFound("Key not found");
            }
            return Ok(value.ToString());
        }
    }
}
