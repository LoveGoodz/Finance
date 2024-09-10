using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin request)
        {
            // Sabit kullanıcı adı ve şifre kontrolü (admin - 123456)
            if (request.Username == "admin" && request.Password == "123456")
            {
                // JWT Token oluşturma işlemi
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]); // Kullanıcı anahtarını doğru alın

                // Claim'ler ile token bilgisi ekleyebilirsiniz (örn: roller)
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Benzersiz bir token kimliği
                    new Claim(ClaimTypes.Role, "Admin") // Örneğin, kullanıcı rolü ekleyebilirsiniz
                };

                // Token süresi ve imzalama işlemi
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"])), // Token süresi
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                try
                {
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // Token döndürülür
                    Log.Information("User {Username} successfully logged in", request.Username);
                    return Ok(new { Token = tokenString });
                }
                catch (Exception ex)
                {
                    // Token oluşturulurken bir hata oluştu
                    Log.Error(ex, "Error occurred while creating token for user {Username}", request.Username);
                    return StatusCode(500, $"Token oluşturulurken bir hata oluştu: {ex.Message}");
                }
            }

            // Hatalı kullanıcı adı veya şifre
            Log.Warning("Invalid login attempt for user {Username}", request.Username);
            return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
        }

        // Basit model sınıfı
        public class UserLogin
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
