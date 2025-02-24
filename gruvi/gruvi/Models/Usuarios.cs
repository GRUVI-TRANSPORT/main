namespace gruvi.Models;
using Microsoft.EntityFrameworkCore;
using gruvi.Pages;
using System;
using System.ComponentModel.DataAnnotations;

public class Usuarios
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nombre { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    [StringLength(255)]
    public string? Password { get; set; }

    public DateTime FechaRegistro { get; set; }
}