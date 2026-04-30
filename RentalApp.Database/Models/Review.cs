using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RentalApp.Database.Models;

[Table("reviews")]
[PrimaryKey(nameof(Id))]
public class Review
{
    public int Id { get; set; }

    [Required]
    public int RentalId { get; set; }

    [Required]
    public int ReviewerId { get; set; }

    public string ReviewerName { get; set; } = string.Empty;

    [Required]
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;
}
