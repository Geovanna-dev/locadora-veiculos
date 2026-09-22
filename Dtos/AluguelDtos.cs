using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public class AluguelCreateDto
{
    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int VeiculoId { get; set; }

    [Required]
    public DateTime DataRetirada { get; set; }

    [Required]
    public DateTime DataPrevistaDevolucao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária deve ser maior que zero.")]
    public decimal ValorDiaria { get; set; }
}

// Usado no endpoint específico de devolução (PUT /api/alugueis/{id}/devolucao)
public class AluguelDevolucaoDto
{
    [Required]
    public DateTime DataDevolucao { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int QuilometragemFinal { get; set; }
}

public class AluguelResponseDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int VeiculoId { get; set; }
    public string VeiculoModelo { get; set; } = string.Empty;
    public string VeiculoPlaca { get; set; } = string.Empty;
    public DateTime DataRetirada { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public int QuilometragemInicial { get; set; }
    public int? QuilometragemFinal { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal? ValorTotal { get; set; }
}