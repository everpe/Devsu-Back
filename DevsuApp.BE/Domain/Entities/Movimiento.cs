using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevsuApp.BE.Domain.Enums;

namespace DevsuApp.BE.Domain.Entities;

[Table("Movimientos")]
public class Movimiento
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
    public TipoMovimiento TipoMovimiento { get; set; }

    [Required(ErrorMessage = "El valor es obligatorio")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Saldo { get; set; }

    // Foreign Key
    [Required(ErrorMessage = "La cuenta es obligatoria")]
    public int CuentaId { get; set; }

    // Navigation Property
    [ForeignKey("CuentaId")]
    public virtual Cuenta Cuenta { get; set; } = null!;
}