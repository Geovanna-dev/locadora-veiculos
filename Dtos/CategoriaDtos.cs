using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Descricao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária base deve ser maior que zero.")]
    public decimal ValorDiariaBase { get; set; }
}

public class CategoriaUpdateDto : CategoriaCreateDto
{
}

public class CategoriaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorDiariaBase { get; set; }
}