using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Models;

public class Veiculo
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    // Quilometragem atual do veículo (atualizada na devolução de cada aluguel)
    public int Quilometragem { get; set; }

    // Somente letras e números, sem hífen (ex.: ABC1D23)
    [Required, StringLength(7, MinimumLength = 7)]
    public string Placa { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Cor { get; set; }

    public StatusVeiculo Status { get; set; } = StatusVeiculo.Disponivel;

    // Chaves estrangeiras + navegações
    [ForeignKey(nameof(Fabricante))]
    public int FabricanteId { get; set; }
    public Fabricante Fabricante { get; set; } = null!;

    [ForeignKey(nameof(Categoria))]
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    // Um veículo pode ter vários aluguéis ao longo do tempo
    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}