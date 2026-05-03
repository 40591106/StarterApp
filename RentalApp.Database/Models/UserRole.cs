using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RentalApp.Database.Models;

[Table("user_role")]
[PrimaryKey(nameof(Id))]
public class UserRole
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoleId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public UserRole() { }

    public UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
        IsActive = true;
    }
}