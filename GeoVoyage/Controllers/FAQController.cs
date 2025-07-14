using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public FAQController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQ>>> GetFAQs()
        {
            try
            {
                var faqs = await _context.FAQ
                    .Where(f => f.IsActive)
                    .OrderBy(f => f.DisplayOrder)
                    .ThenBy(f => f.Category)
                    .ToListAsync();

                return Ok(faqs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<FAQ>>> GetFAQsByCategory(string category)
        {
            try
            {
                var faqs = await _context.FAQ
                    .Where(f => f.IsActive && f.Category.ToLower() == category.ToLower())
                    .OrderBy(f => f.DisplayOrder)
                    .ToListAsync();

                return Ok(faqs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FAQ>> CreateFAQ(FAQ faq)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.FAQ.Add(faq);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFAQs), new { id = faq.Id }, faq);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFAQ(int id, FAQ faq)
        {
            if (id != faq.Id)
            {
                return BadRequest("FAQ ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(faq).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(faq);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FAQExists(id))
                {
                    return NotFound($"FAQ with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            try
            {
                var faq = await _context.FAQ.FindAsync(id);
                if (faq == null)
                {
                    return NotFound($"FAQ with ID {id} not found.");
                }

                // Soft delete
                faq.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok("FAQ deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool FAQExists(int id)
        {
            return _context.FAQ.Any(e => e.Id == id);
        }
    }
}