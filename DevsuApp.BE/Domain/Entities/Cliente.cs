using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Domain.Entities;

[Table("Clientes")]
[Index(nameof(Identificacion), IsUnique = true)]
public class Cliente : Persona
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 100 caracteres")]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    public bool Estado { get; set; } = true;

    // Relación 1:N con Cuenta
    public virtual ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}