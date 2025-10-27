using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevsuApp.BE.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Domain.Entities;

[Table("Cuentas")]
[Index(nameof(NumeroCuenta), IsUnique = true)] 
public class Cuenta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de cuenta es obligatorio")]
    [StringLength(20, ErrorMessage = "El número de cuenta no puede exceder 20 caracteres")]
    public string NumeroCuenta { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de cuenta es obligatorio")]
    public TipoCuenta TipoCuenta { get; set; }

    [Required(ErrorMessage = "El saldo inicial es obligatorio")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SaldoInicial { get; set; }

    [Required]
    public bool Estado { get; set; } = true;

    // Foreign Key
    [Required(ErrorMessage = "El cliente es obligatorio")]
    public int ClienteId { get; set; }

    // Navigation Property
    [ForeignKey("ClienteId")]
    public virtual Cliente Cliente { get; set; } = null!;

    // Relación 1:N con Movimiento
    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}