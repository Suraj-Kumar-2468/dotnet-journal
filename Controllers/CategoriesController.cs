using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private static List<Category> _catg = new();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<List<Category>> getall([FromQuery] string? search)
        
        {
            if (!string.IsNullOrEmpty(search))
            {
                return _catg.Where(item => item.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return _catg;
        }

        [HttpGet("{id}")]
        public ActionResult<Category> getbyid(int id)
        {
            Category categ = _catg.FirstOrDefault(item => item.Id == id);
            if (categ is null)
            {
                return NotFound();
            }
            return categ;
        }
        [HttpPost]
        public IActionResult createcategory(CreateCategoryRequest request)
        {
            Category isExist = _catg.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase));
            if(isExist is not null)
                return Conflict();
            Category body = new Category(_nextId++, request.Name);
            _catg.Add(body);
            return CreatedAtAction(nameof(getbyid), new { id = body.Id }, body);
        }

        [HttpPost("createmultiple")]
        public IActionResult createmultiplecategories(List<CreateCategoryRequest> requests)
        {
            var createdCategories = new List<Category>();
            foreach (var request in requests)
            {
                Category isExist = _catg.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase));
                if(isExist is not null)
                    return Conflict();
                Category body = new Category(_nextId++, request.Name);
                _catg.Add(body);
                createdCategories.Add(body);
            }
            return Ok(createdCategories);
        }

        [HttpDelete("{id}")]
        public IActionResult deletecatgory(int id)
        {
            Category categ = _catg.FirstOrDefault(item => item.Id == id);
            if(categ is null)
            {
                return NotFound();
            }
            _catg.Remove(categ);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult updatecategory(int id, UpdateCategoryRequest request)
        {
            Category categ = _catg.FirstOrDefault(item => item.Id == id);
            if(categ is null)
                return NotFound();
            Category isExist = _catg.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase) && item.Id != id);
            if(isExist is not null)
                return Conflict();
            var updatedCategory = new Category(categ.Id, request.Name);
            _catg[_catg.IndexOf(categ)] = updatedCategory;
            categ = updatedCategory;
            return Ok(categ);
        }

        [HttpPatch("{id}")]
        public IActionResult patchcategory(int id, UpdateCategoryRequest request)
        {
            Category categ = _catg.FirstOrDefault(item => item.Id == id);
            if(categ is null)
                return NotFound();
            if(!string.IsNullOrEmpty(request.Name))
            {
                Category isExist = _catg.FirstOrDefault(item => string.Equals(item.Name, request.Name, StringComparison.OrdinalIgnoreCase) && item.Id != id);
                if(isExist is not null)
                    return Conflict();
                categ = new Category(categ.Id, request.Name);
                _catg[_catg.IndexOf(_catg.First(item => item.Id == id))] = categ;
            }
            return Ok(categ);
        }


    }


}
