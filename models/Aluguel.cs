using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Models;

public class Aluguel
{
    [Key]
    public int Id { get; set; }

    // Chaves estrangeiras + navegações
    [ForeignKey(nameof(Cliente))]
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    [ForeignKey(nameof(Veiculo))]
    public int VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; } = null!;

    // Período da locação
    public DateTime DataRetirada { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }

    // Registro da devolução: nulo enquanto o veículo não foi devolvido
    public DateTime? DataDevolucao { get; set; }

    public int QuilometragemInicial { get; set; }
    public int? QuilometragemFinal { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorDiaria { get; set; }

    // Calculado na devolução: dias x ValorDiaria
    [Column(TypeName = "decimal(10,2)")]
    public decimal? ValorTotal { get; set; }
}