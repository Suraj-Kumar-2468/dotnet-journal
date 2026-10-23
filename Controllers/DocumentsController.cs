using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private static readonly List<Document> _docs = new();
    private static int _nextId = 1;
    private int id;


    [HttpGet]
    public ActionResult<List<Document>> GetAll() => _docs;

    [HttpGet("{id}")]
    public ActionResult<Document> GetById(int Id)
    {
        var doc = _docs.FirstOrDefault(d => d.Id == id);
        return doc is null ? NotFound() : doc;
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

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var doc = _docs.FirstOrDefault(d => d.Id == id);
        if (doc is null) return NotFound();
        _docs.Remove(doc);
        return NoContent();
    }


}
