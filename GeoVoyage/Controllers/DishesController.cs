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
    public class DishesController : ControllerBase
    {
        private readonly GeoVoyageDbContext _context;

        public DishesController(GeoVoyageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes()
        {
            try
            {
                var dishes = await _context.Dishes
                    .OrderBy(p => p.Name)
                    .ToListAsync();
                return Ok(dishes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            try
            {
                var dishes = await _context.Dishes.FindAsync(id);

                if (dishes == null)
                {
                    return NotFound($"Dish with ID {id} not found.");
                }

                return Ok(dishes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishesByCategory(string category)
        {
            try
            {
                var dishes = await _context.Dishes
                    .Where(p => p.Category.ToLower() == category.ToLower())
                    .ToListAsync();

                return Ok(dishes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("instock")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetInStockDishes()
        {
            try
            {
                var dishes = await _context.Dishes
                    .ToListAsync();

                return Ok(dishes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Dish>> CreateDish(Dish dish)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDish(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest("Dish ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(dish).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(dish);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                {
                    return NotFound($"Dish with ID {id} not found.");
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
        public async Task<IActionResult> DeleteDish(int id)
        {
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                {
                    return NotFound($"Dish with ID {id} not found.");
                }

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                return Ok($"Dish '{dish.Name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}