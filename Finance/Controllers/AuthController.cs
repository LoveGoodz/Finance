using Finance.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            // Servis ile kullanıcı doğrulama işlemi
            var user = await _authService.AuthenticateAsync(request.Username, request.Password);

            if (user != null)
            {
                // Token oluşturma işlemi
                var token = _authService.GenerateToken(user);
                Log.Information("Başarılı giriş: {Username}", request.Username);
                return Ok(new { Token = token });
            }

            Log.Warning("Başarısız giriş denemesi: {Username}", request.Username);
            return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
        }

        public class UserLogin
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
