using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoVoyage.Data;
using GeoVoyage.Models;

namespace GeoVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourPackagesController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public TourPackagesController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourPackage>>> GetTourPackages()
        {
            try
            {
                var packages = await _context.TourPackages
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TourPackage>> GetTourPackage(int id)
        {
            try
            {
                var package = await _context.TourPackages.FindAsync(id);

                if (package == null || !package.IsActive)
                {
                    return NotFound($"Tour package with ID {id} not found.");
                }

                return Ok(package);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TourPackage>>> SearchPackages([FromQuery] string query)
        {
            try
            {
                var packages = await _context.TourPackages
                    .Where(p => p.IsActive &&
                           (p.Name.Contains(query) ||
                            p.Description.Contains(query) ||
                            p.Difficulty.Contains(query)))
                    .ToListAsync();

                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TourPackage>> CreateTourPackage(TourPackage package)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.TourPackages.Add(package);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTourPackage), new { id = package.Id }, package);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTourPackage(int id, TourPackage package)
        {
            if (id != package.Id)
            {
                return BadRequest("Package ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(package).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(package);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
                {
                    return NotFound($"Tour package with ID {id} not found.");
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
        public async Task<IActionResult> DeleteTourPackage(int id)
        {
            try
            {
                var package = await _context.TourPackages.FindAsync(id);
                if (package == null)
                {
                    return NotFound($"Tour package with ID {id} not found.");
                }

                // Soft delete
                package.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok($"Tour package '{package.Name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool PackageExists(int id)
        {
            return _context.TourPackages.Any(e => e.Id == id);
        }
    }
}