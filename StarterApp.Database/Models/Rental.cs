using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StarterApp.Database.Models;

[Table("rentals")]
[PrimaryKey(nameof(Id))]
public class Rental
{
    public int Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }
    public bool IsPending => Status == "Requested";
    public bool IsApproved => Status == "Approved";
    public bool IsOutForRent => Status == "Out for Rent";
    public bool IsReturned => Status == "Returned";
    public bool IsOverdue => Status == "Out for Rent" && EndDate < DateTime.UtcNow;
    public bool IsCompleted => Status == "Completed";
    public string ItemTitle { get; set; } = string.Empty;

    [Required]
    public int BorrowerId { get; set; }

    [ForeignKey(nameof(BorrowerId))]
    public User? Borrower { get; set; }

    public string BorrowerName { get; set; } = string.Empty;

    [Required]
    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string Status { get; set; } = "Requested";
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
