
using System.ComponentModel.DataAnnotations;
namespace DocAssistant.Api.Models;

public record CreateDocumentRequest(
    [Required] [MaxLength(100)] string Title,
    [Required] [MaxLength(1000)] string Content
);

public record UpdateDocumentRequest(
    [MinLength(1)] [MaxLength(100)] string? Title,
    [MinLength(1)] [MaxLength(1000)] string? Content
);