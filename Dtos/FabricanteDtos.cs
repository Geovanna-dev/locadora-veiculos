using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public class FabricanteCreateDto
{
    [Required(ErrorMessage = "O nome do fabricante é obrigatório.")]
    [MaxLength(80)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(60)]
    public string? PaisOrigem { get; set; }
}

public class FabricanteUpdateDto : FabricanteCreateDto
{
}

public class FabricanteResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? PaisOrigem { get; set; }
    public int QuantidadeVeiculos { get; set; }
}