using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlugueisController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AlugueisController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/alugueis
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AluguelResponseDto>>> GetAll()
    {
        var alugueis = await ProjetarAlugueis(_context.Alugueis)
            .OrderByDescending(a => a.DataRetirada)
            .ToListAsync();

        return Ok(alugueis);
    }

    // GET: api/alugueis/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AluguelResponseDto>> GetById(int id)
    {
        var aluguel = await ProjetarAlugueis(_context.Alugueis.Where(a => a.Id == id))
            .FirstOrDefaultAsync();

        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com id {id} não encontrado." });

        return Ok(aluguel);
    }

    // FILTRO: aluguéis em aberto (ainda não devolvidos) — usa Include (LEFT JOIN)
    [HttpGet("em-aberto")]
    public async Task<ActionResult<IEnumerable<AluguelResponseDto>>> GetEmAberto()
    {
        var alugueis = await _context.Alugueis
            .Include(a => a.Cliente)
            .Include(a => a.Veiculo)
            .Where(a => a.DataDevolucao == null)
            .OrderBy(a => a.DataPrevistaDevolucao)
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

        return Ok(alugueis);
    }

    // FILTRO: aluguéis retirados dentro de um período — usa join explícito (INNER JOIN)
    [HttpGet("por-periodo")]
    public async Task<ActionResult<IEnumerable<AluguelResponseDto>>> GetPorPeriodo(
        [FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        if (fim < inicio)
            return BadRequest(new { mensagem = "A data final não pode ser anterior à data inicial." });

        var alugueis = await (
            from a in _context.Alugueis
            join c in _context.Clientes on a.ClienteId equals c.Id
            join v in _context.Veiculos on a.VeiculoId equals v.Id
            where a.DataRetirada >= inicio && a.DataRetirada <= fim
            orderby a.DataRetirada
            select new AluguelResponseDto
            {
                Id = a.Id,
                ClienteId = c.Id,
                ClienteNome = c.Nome,
                VeiculoId = v.Id,
                VeiculoModelo = v.Modelo,
                VeiculoPlaca = v.Placa,
                DataRetirada = a.DataRetirada,
                DataPrevistaDevolucao = a.DataPrevistaDevolucao,
                DataDevolucao = a.DataDevolucao,
                QuilometragemInicial = a.QuilometragemInicial,
                QuilometragemFinal = a.QuilometragemFinal,
                ValorDiaria = a.ValorDiaria,
                ValorTotal = a.ValorTotal
            }
        ).ToListAsync();

        return Ok(alugueis);
    }

    // POST: api/alugueis — abre uma locação
    [HttpPost]
    public async Task<ActionResult<AluguelResponseDto>> Create(AluguelCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.DataPrevistaDevolucao < dto.DataRetirada)
            return BadRequest(new { mensagem = "A data prevista de devolução não pode ser anterior à data de retirada." });

        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente is null)
            return BadRequest(new { mensagem = $"Cliente com id {dto.ClienteId} não existe." });

        var veiculo = await _context.Veiculos.FindAsync(dto.VeiculoId);
        if (veiculo is null)
            return BadRequest(new { mensagem = $"Veículo com id {dto.VeiculoId} não existe." });

        if (veiculo.Status != StatusVeiculo.Disponivel)
            return Conflict(new { mensagem = "Este veículo não está disponível para locação no momento." });

        var aluguel = new Aluguel
        {
            ClienteId = dto.ClienteId,
            VeiculoId = dto.VeiculoId,
            DataRetirada = dto.DataRetirada,
            DataPrevistaDevolucao = dto.DataPrevistaDevolucao,
            QuilometragemInicial = veiculo.Quilometragem,
            ValorDiaria = dto.ValorDiaria
        };

        veiculo.Status = StatusVeiculo.Alugado;
        _context.Alugueis.Add(aluguel);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não foi possível registrar o aluguel. Verifique os dados informados." });
        }

        var response = await ProjetarAlugueis(_context.Alugueis.Where(a => a.Id == aluguel.Id)).FirstAsync();
        return CreatedAtAction(nameof(GetById), new { id = aluguel.Id }, response);
    }

    // PUT: api/alugueis/5/devolucao — fecha a locação
    [HttpPut("{id:int}/devolucao")]
    public async Task<IActionResult> RegistrarDevolucao(int id, AluguelDevolucaoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var aluguel = await _context.Alugueis.Include(a => a.Veiculo).FirstOrDefaultAsync(a => a.Id == id);
        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com id {id} não encontrado." });

        if (aluguel.DataDevolucao is not null)
            return Conflict(new { mensagem = "Este aluguel já foi devolvido." });

        if (dto.DataDevolucao < aluguel.DataRetirada)
            return BadRequest(new { mensagem = "A data de devolução não pode ser anterior à data de retirada." });

        if (dto.QuilometragemFinal < aluguel.QuilometragemInicial)
            return BadRequest(new { mensagem = "A quilometragem final não pode ser menor que a inicial." });

        var dias = Math.Max(1, (dto.DataDevolucao.Date - aluguel.DataRetirada.Date).Days);

        aluguel.DataDevolucao = dto.DataDevolucao;
        aluguel.QuilometragemFinal = dto.QuilometragemFinal;
        aluguel.ValorTotal = dias * aluguel.ValorDiaria;

        aluguel.Veiculo.Quilometragem = dto.QuilometragemFinal;
        aluguel.Veiculo.Status = StatusVeiculo.Disponivel;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/alugueis/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var aluguel = await _context.Alugueis.Include(a => a.Veiculo).FirstOrDefaultAsync(a => a.Id == id);
        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com id {id} não encontrado." });

        if (aluguel.DataDevolucao is null)
            aluguel.Veiculo.Status = StatusVeiculo.Disponivel;

        _context.Alugueis.Remove(aluguel);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static IQueryable<AluguelResponseDto> ProjetarAlugueis(IQueryable<Aluguel> query)
    {
        return query
            .Include(a => a.Cliente)
            .Include(a => a.Veiculo)
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
            });
    }
}