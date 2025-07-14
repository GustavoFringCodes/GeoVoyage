using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;
using System.Security.Cryptography;
using System.Text;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountsController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public CustomerAccountsController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if email already exists
                var existingUser = await _context.CustomerAccounts
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email already registered.");
                }

                // Hash password
                var passwordHash = HashPassword(request.Password);

                var customer = new CustomerAccount
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CustomerAccounts.Add(customer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registration successful", customerId = customer.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = await _context.CustomerAccounts
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (customer == null || !VerifyPassword(request.Password, customer.PasswordHash))
                {
                    return Unauthorized("Invalid email or password.");
                }

                if (!customer.IsActive)
                {
                    return Unauthorized("Account is deactivated.");
                }

                // Generate session token
                var sessionToken = GenerateSessionToken();
                var session = new UserSession
                {
                    CustomerAccountId = customer.Id,
                    SessionToken = sessionToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Login successful",
                    token = sessionToken,
                    customer = new
                    {
                        id = customer.Id,
                        email = customer.Email,
                        firstName = customer.FirstName,
                        lastName = customer.LastName,
                        phone = customer.Phone
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        private string GenerateSessionToken()
        {
            return Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks.ToString();
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}