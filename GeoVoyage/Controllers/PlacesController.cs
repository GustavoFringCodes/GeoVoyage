using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public PlacesController(GeoVoyageDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            try
            {
                var places = await _context.Places
                    .OrderBy(p => p.Name)
                    .ToListAsync();
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Place>> GetPlace(int id)
        {
            try
            {
                var places = await _context.Places.FindAsync(id);

                if (places == null)
                {
                    return NotFound($"Place with ID {id} not found.");
                }

                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Place>> CreatePlace(Place place)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Places.Add(place);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPlace), new { id = place.Id }, place);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(int id, Place place)
        {
            if (id != place.Id)
            {
                return BadRequest("Place ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(place).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(place);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
                {
                    return NotFound($"Place with ID {id} not found.");
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
        public async Task<IActionResult> DeletePlace(int id)
        {
            try
            {
                var place = await _context.Places.FindAsync(id);
                if (place == null)
                {
                    return NotFound($"Place with ID {id} not found.");
                }

                _context.Places.Remove(place);
                await _context.SaveChangesAsync();

                return Ok($"Place '{place.Name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(t => t.Id == id);
        }
    }
}
