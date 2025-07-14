using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public ContactController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpPost("message")]
        public async Task<ActionResult> SendMessage([FromBody] ContactMessageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var contactMessage = new ContactMessage
                {
                    Name = request.Name,
                    Email = request.Email,
                    Subject = request.Subject,
                    Message = request.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Your message has been sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("newsletter")]
        public async Task<ActionResult> Subscribe([FromBody] NewsletterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if already subscribed
                var existing = await _context.NewsletterSubscriptions
                    .FirstOrDefaultAsync(n => n.Email == request.Email);

                if (existing != null)
                {
                    if (existing.IsActive)
                    {
                        return BadRequest("Email is already subscribed to newsletter.");
                    }
                    else
                    {
                        // Reactivate subscription
                        existing.IsActive = true;
                        existing.SubscribedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return Ok(new { message = "Newsletter subscription reactivated!" });
                    }
                }

                var subscription = new NewsletterSubscription
                {
                    Email = request.Email,
                    SubscribedAt = DateTime.UtcNow
                };

                _context.NewsletterSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Successfully subscribed to newsletter!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("messages")]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetMessages()
        {
            try
            {
                var messages = await _context.ContactMessages
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class ContactMessageRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; }
    }

    public class NewsletterRequest
    {
        public string Email { get; set; }
    }
}