using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Models;

public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    // Somente os 11 dígitos, sem pontos ou traço
    [Required, StringLength(11, MinimumLength = 11)]
    public string Cpf { get; set; } = string.Empty;

    [Required, MaxLength(150), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; set; }

    // Um cliente pode ter vários aluguéis
    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}