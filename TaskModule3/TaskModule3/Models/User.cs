using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TaskModule3.Models;

[Table("users")]
[Index("Login", Name = "users_login_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("surname")]
    [StringLength(255)]
    public string Surname { get; set; } = null!;

    [Column("patronymic")]
    [StringLength(255)]
    public string? Patronymic { get; set; }

    [Column("login")]
    [StringLength(255)]
    public string Login { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    [Column("last_enter", TypeName = "timestamp without time zone")]
    public DateTime? LastEnter { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }
}
