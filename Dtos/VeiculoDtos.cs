using System.ComponentModel.DataAnnotations;
using Locadora.Api.Models;

namespace Locadora.Api.Dtos;

public class VeiculoCreateDto
{
    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [MaxLength(80)]
    public string Modelo { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Ano de fabricação inválido.")]
    public int AnoFabricacao { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    public int Quilometragem { get; set; }

    [Required(ErrorMessage = "A placa é obrigatória.")]
    [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter exatamente 7 caracteres.")]
    public string Placa { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Cor { get; set; }

    [Required]
    public int FabricanteId { get; set; }

    [Required]
    public int CategoriaId { get; set; }
}

// Na atualização também é possível alterar o status manualmente (ex.: colocar em manutenção)
public class VeiculoUpdateDto : VeiculoCreateDto
{
    [Required]
    public StatusVeiculo Status { get; set; }
}

public class VeiculoResponseDto
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public int AnoFabricacao { get; set; }
    public int Quilometragem { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string? Cor { get; set; }
    public StatusVeiculo Status { get; set; }
    public int FabricanteId { get; set; }
    public string FabricanteNome { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
}