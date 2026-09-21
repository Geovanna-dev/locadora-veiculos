using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Models;

public class Fabricante
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(60)]
    public string? PaisOrigem { get; set; }

    // Navegação: um fabricante possui vários veículos
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}