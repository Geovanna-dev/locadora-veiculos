using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Models;

public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Descricao { get; set; }

    // Valor de referência da diária. O valor efetivamente cobrado fica em Aluguel.ValorDiaria
    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorDiariaBase { get; set; }

    // Navegação: uma categoria classifica vários veículos
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}