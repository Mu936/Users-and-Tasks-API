using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersAndTasksAPI.Models;
using UsersAndTasksAPI.Models.Responses;
using UsersAndTasksAPI.Data;
using UsersAndTasksAPI.Repositories;
using UsersAndTasksAPI.Services;

namespace UsersAndTasksAPI.Controllers
{
    /// <summary>
    /// Controller handling user authentication and registration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous] // Allow unauthenticated access to auth endpoints
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public AuthController(
            IConfiguration configuration, 
            AppDbContext context, 
            IUserRepository userRepository,
            IPasswordService passwordService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="registration">User registration details</param>
        /// <returns>JWT token for the registered user</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="400">If the username is already taken</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody, Required] UserRegistrationDto registration)
        {
            try
            {
                // Check if username is already taken
                if (await _userRepository.GetUserByUsernameAsync(registration.Username) != null)
                {
                    return BadRequest(new { message = "Username is already taken" });
                }

                // Create new user with hashed password
                var user = new User
                {
                    Username = registration.Username.Trim(),
                    Password = _passwordService.HashPassword(registration.Password),
                    Email = registration.Email.Trim()
                };

                // Save user to database
                var createdUser = await _userRepository.AddUserAsync(user);

                // Generate JWT token
                var token = GenerateJwtToken(createdUser);

                return Ok(new AuthResponse
                {
                    Token = token,
                    ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60") * 60, // Convert to seconds
                    Username = createdUser.Username,
                    UserId = createdUser.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user", error = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            
            if (!int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var expiresInMinutes))
            {
                expiresInMinutes = 60; // Default to 60 minutes if not specified
            }
            
            var issuer = _configuration["Jwt:Issuer"] ?? "UsersTasksAPI";
            var audience = _configuration["Jwt:Audience"] ?? "UsersTasksAPIUsers";
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="login">User login credentials</param>
        /// <returns>JWT token for the authenticated user</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">If the credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody, Required] UserLoginDto login)
        {
            var user = await _userRepository.GetUserByUsernameAsync(login.Username);
            if (user == null || !_passwordService.VerifyPassword(login.Password, user.Password))
                return Unauthorized(new { message = "Invalid username or password." });

            var token = GenerateJwtToken(user);
            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60") * 60, // Convert to seconds
                Username = user.Username,
                UserId = user.Id
            });
        }
    }
}
