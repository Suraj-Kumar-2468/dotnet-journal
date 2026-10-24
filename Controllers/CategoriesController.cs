using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private static List<Category> _categories = new();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<List<Category>> GetAll([FromQuery] string? search)
        
        {
            if (!string.IsNullOrEmpty(search))
            {
                return _categories.Where(item => item.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return _categories;
        }

        [HttpGet("{id}")]
        public ActionResult<Category> GetById(int id)
        {
            Category? category = _categories.FirstOrDefault(item => item.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return category;
        }
        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryRequest request)
        {
            Category? isExist = _categories.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase));
            if(isExist is not null)
                return Conflict();
            Category body = new Category(_nextId++, request.Name);
            _categories.Add(body);
            return CreatedAtAction(nameof(GetById), new { id = body.Id }, body);
        }

        [HttpPost("bulk")]
        public IActionResult CreateMultipleCategories(List<CreateCategoryRequest> requests)
        {
            var createdCategories = new List<Category>();
            foreach (var request in requests)
            {
                Category? isExist = _categories.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase));
                if(isExist is not null)
                    return Conflict();
                Category body = new Category(_nextId++, request.Name);
                _categories.Add(body);
                createdCategories.Add(body);
            }
            return Ok(createdCategories);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            Category? category = _categories.FirstOrDefault(item => item.Id == id);
            if(category is null)
            {
                return NotFound();
            }
            _categories.Remove(category);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, UpdateCategoryRequest request)
        {
            Category? category = _categories.FirstOrDefault(item => item.Id == id);
            if(category is null)
                return NotFound();
            Category? existing = _categories.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase) && item.Id != id);
            if(existing is not null)
                return Conflict();
            var updatedCategory = new Category(category.Id, request.Name);
           
            _categories[_categories.IndexOf(category)] = updatedCategory;
            category = updatedCategory;
            return Ok(category);
        }

    }


}
