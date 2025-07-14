using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public BookingsController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var booking = new Booking
                {
                    CustomerAccountId = request.CustomerAccountId,
                    CustomerName = request.CustomerName,
                    Email = request.Email,
                    Phone = request.Phone,
                    DestinationId = request.DestinationId,
                    PackageType = request.PackageType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    NumberOfGuests = request.NumberOfGuests,
                    TotalPrice = request.TotalPrice,
                    SpecialRequests = request.SpecialRequests,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking created successfully!", bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.CustomerAccount)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.CustomerAccount)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetCustomerBookings(string email)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.Email == email)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateBookingStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                booking.Status = request.Status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking status updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class BookingRequest
    {
        public int? CustomerAccountId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public int? DestinationId { get; set; }
        public string? PackageType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class StatusUpdateRequest
    {
        public string Status { get; set; }
    }
}