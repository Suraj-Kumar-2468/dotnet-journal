namespace DocAssistant.Api.Models;

public record CreateDocumentRequest(string Title, string Content);

public class UpdateDocumentRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
}
