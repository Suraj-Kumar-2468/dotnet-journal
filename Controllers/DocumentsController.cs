using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private static readonly List<Document> _docs = new();
    private static int _nextId = 1;

    [HttpGet()]
    public ActionResult<List<Document>> GetAll([FromQuery(Name = "search")] string? search)
    {
        var docs = _docs.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            docs = docs.Where(d => d.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || d.Content.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        return docs.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Document> GetById(int id)
    {
        var doc = _docs.FirstOrDefault(d => d.Id == id);
        return doc is null ? NotFound() : doc;
    }

    [HttpPost("bulk")]
    public ActionResult<List<Document>> CreateBulk(List<CreateDocumentRequest> request)
    {
        var docs = new List<Document>();
        foreach (var item in request)
        {
            var doc = new Document
            {
                Id = _nextId++,
                Title = item.Title,
                Content = item.Content,
                CreatedAt = DateTime.UtcNow
            };
            _docs.Add(doc);
            docs.Add(doc);
        }
        return CreatedAtAction(nameof(GetAll), new { search = string.Join(",", request.Select(r => r.Title)) }, docs);
    }

    [HttpPost]
    public ActionResult<Document> Create(CreateDocumentRequest request)
    {
        var doc = new Document
        {
            Id = _nextId++,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _docs.Add(doc);
        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, doc);
    }

    [HttpPatch("{id}")]
    public ActionResult<Document> Update(int id, UpdateDocumentRequest request)
    {   
        if (request.Title is null && request.Content is null)
            return BadRequest("Nothing to update");

        var doc = _docs.FindIndex(d => d.Id == id);
        if (doc is -1) return NotFound();
        _docs[doc] = new Document
        {
            Id =  _docs[doc].Id,
            Title = request?.Title is null ? _docs[doc].Title : request?.Title, 
            Content = request?.Content is null ? _docs[doc].Content : request?.Content,
            CreatedAt = _docs[doc].CreatedAt
        };
        return Ok(_docs[doc]);
    } 

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var doc = _docs.FirstOrDefault(d => d.Id == id);
        if (doc is null) return NotFound();
        _docs.Remove(doc);
        return NoContent();
    }


}