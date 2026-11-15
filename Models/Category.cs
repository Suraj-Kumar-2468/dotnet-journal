using System;
using System.ComponentModel.DataAnnotations;

namespace DocAssistant.Api.Models;

// public record Category(
//     int Id,
//     string Name
// );

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }    = "";
}

public record CreateCategoryRequest(
    [Required] [MaxLength(50)] string Name
);
public record UpdateCategoryRequest(
    [Required] [MaxLength(50)] string Name
);
    