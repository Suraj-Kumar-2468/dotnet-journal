using System;
using System.ComponentModel.DataAnnotations;

namespace DocAssistant.Api.Models;

public record Category(
    int Id,
    string Name
);

public record CreateCategoryRequest(
    [Required] [MaxLength(50)] string Name
);
public record UpdateCategoryRequest(
    [Required] [MaxLength(50)] string Name
);
    