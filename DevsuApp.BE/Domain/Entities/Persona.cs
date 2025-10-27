using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevsuApp.BE.Domain.Entities;

public class Persona
{
    public int PersonaId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio")]
    [StringLength(10, ErrorMessage = "El género no puede exceder 10 caracteres")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
    public int Edad { get; set; }

    [Required(ErrorMessage = "La identificación es obligatoria")]
    [StringLength(20, ErrorMessage = "La identificación no puede exceder 20 caracteres")]
    public string Identificacion { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Telefono { get; set; }
}