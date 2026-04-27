using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StarterApp.Database.Models;

[Table("items")]
[PrimaryKey(nameof(Id))]
public class Item
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal DailyRate { get; set; }

    [Required]
    public int CategoryId { get; set; }
    public string? Category { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? CategoryNavigation { get; set; }
    public double? Longitude { get; set; } = 0.0;
    public double? Latitude { get; set; } = 0.0;
    public bool IsAvailable { get; set; } = true;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;
}
