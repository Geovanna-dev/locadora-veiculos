using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public class ClienteCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 dígitos.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter apenas números, sem pontos ou traço.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; set; }
}

public class ClienteUpdateDto : ClienteCreateDto
{
}

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
}