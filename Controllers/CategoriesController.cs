using DocAssistant.Api.Data;
using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAll([FromQuery] string? search)
        
        {
            if (!string.IsNullOrEmpty(search))
            {
                return await _context.Categories.Where(item => EF.Functions.Like(item.Name, $"%{search}%")).ToListAsync();
            }
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return category;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            bool exists = await _context.Categories.AnyAsync(c => EF.Functions.Like(c.Name, request.Name));
            if(exists)
                return Conflict($"Category '{request.Name}' already exists");
            Category body = new Category { Name = request.Name }; ;
            _context.Categories.Add(body);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = body.Id }, body);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateMultipleCategories(List<CreateCategoryRequest> requests)
        {
           var names = requests.Select(r => r.Name).ToList();

           if(names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
            {
                return Conflict("Request has duplicate names");
            }

            var lowered = names.Select(n => n.ToLower()).ToList();
            var existing = await _context.Categories.Where(c=> lowered.Contains(c.Name.ToLower())).Select(c=> c.Name).ToListAsync();

            if (existing.Count > 0)
                return Conflict($"Already exist: {string.Join(", ", existing)}");

            var categories = names.Select(n => new Category {Name = n}).ToList();
            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            return StatusCode(201, categories);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if(category is null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if(category is null)
                return NotFound();
            bool exists = await _context.Categories.AnyAsync(c => c.Id != id && EF.Functions.Like(c.Name, request.Name));
            if(exists)
                return Conflict($"Category '{request.Name}' already exists");
            category.Name = request.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }


}
