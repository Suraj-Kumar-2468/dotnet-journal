using DocAssistant.Api.Data;
using DocAssistant.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DocAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Document>>> GetAll([FromQuery(Name = "search")] string? search)
    {
        var docs = _context.Documents.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            docs = docs.Where(d => EF.Functions.Like(d.Title, $"%{search.Trim()}%") || EF.Functions.Like(d.Content.Trim(), $"%{search.Trim()}%"));
        }
        return await docs.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Document>> GetById(int id)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        return doc is null ? NotFound() : doc;
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<Document>>> CreateBulk(List<CreateDocumentRequest> request)
    {
        var docs = new List<Document>();
        foreach (var item in request)
        {
            var doc = new Document
            {
                Title = item.Title,
                Content = item.Content,
                CreatedAt = DateTime.UtcNow
            };
            docs.Add(doc);
        }
        _context.Documents.AddRange(docs);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { search = string.Join(",", request.Select(r => r.Title)) }, docs);
    }

    [HttpPost]
    public async Task<ActionResult<Document>> Create(CreateDocumentRequest request)
    {
        var doc = new Document
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _context.Documents.Add(doc);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, doc);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Document>> Update(int id, UpdateDocumentRequest request)
    {   
        if (request.Title is null && request.Content is null)
            return BadRequest("Nothing to update");

        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return NotFound();
        doc.Title = request?.Title is null ? doc.Title : request?.Title;
        doc.Content = request?.Content is null ? doc.Content : request?.Content;
        await _context.SaveChangesAsync();
        return Ok(doc);
    } 

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return NotFound();
        _context.Documents.Remove(doc);
        await _context.SaveChangesAsync(); 
        return NoContent();
    }

}