using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ClientesController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetAll()
    {
        var clientes = await _context.Clientes
            .Select(c => new ClienteResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Cpf = c.Cpf,
                Email = c.Email,
                Telefone = c.Telefone
            })
            .ToListAsync();

        return Ok(clientes);
    }

    // GET: api/clientes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> GetById(int id)
    {
        var cliente = await _context.Clientes
            .Where(c => c.Id == id)
            .Select(c => new ClienteResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Cpf = c.Cpf,
                Email = c.Email,
                Telefone = c.Telefone
            })
            .FirstOrDefaultAsync();

        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com id {id} não encontrado." });

        return Ok(cliente);
    }

    // FILTRO 5: histórico de aluguéis de um cliente (Cliente -> Aluguel -> Veiculo).
    // Usa Include (LEFT JOIN) para funcionar mesmo que o cliente nunca tenha alugado nada.
    [HttpGet("{id:int}/historico")]
    public async Task<ActionResult<IEnumerable<AluguelResponseDto>>> GetHistorico(int id)
    {
        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == id);
        if (!clienteExiste)
            return NotFound(new { mensagem = $"Cliente com id {id} não encontrado." });

        var historico = await _context.Alugueis
            .Include(a => a.Veiculo)
            .Include(a => a.Cliente)
            .Where(a => a.ClienteId == id)
            .OrderByDescending(a => a.DataRetirada)
            .Select(a => new AluguelResponseDto
            {
                Id = a.Id,
                ClienteId = a.ClienteId,
                ClienteNome = a.Cliente.Nome,
                VeiculoId = a.VeiculoId,
                VeiculoModelo = a.Veiculo.Modelo,
                VeiculoPlaca = a.Veiculo.Placa,
                DataRetirada = a.DataRetirada,
                DataPrevistaDevolucao = a.DataPrevistaDevolucao,
                DataDevolucao = a.DataDevolucao,
                QuilometragemInicial = a.QuilometragemInicial,
                QuilometragemFinal = a.QuilometragemFinal,
                ValorDiaria = a.ValorDiaria,
                ValorTotal = a.ValorTotal
            })
            .ToListAsync();

        return Ok(historico);
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Create(ClienteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Email = dto.Email,
            Telefone = dto.Telefone
        };

        _context.Clientes.Add(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um cliente cadastrado com esse CPF ou e-mail." });
        }

        var response = new ClienteResponseDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Email = cliente.Email,
            Telefone = cliente.Telefone
        };

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, response);
    }

    // PUT: api/clientes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClienteUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com id {id} não encontrado." });

        cliente.Nome = dto.Nome;
        cliente.Cpf = dto.Cpf;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um cliente cadastrado com esse CPF ou e-mail." });
        }

        return NoContent();
    }

    // DELETE: api/clientes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com id {id} não encontrado." });

        _context.Clientes.Remove(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: este cliente possui aluguéis registrados." });
        }

        return NoContent();
    }
}