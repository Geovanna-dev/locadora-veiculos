using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FabricantesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public FabricantesController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/fabricantes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FabricanteResponseDto>>> GetAll()
    {
        var fabricantes = await _context.Fabricantes
            .Select(f => new FabricanteResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                PaisOrigem = f.PaisOrigem,
                QuantidadeVeiculos = f.Veiculos.Count
            })
            .ToListAsync();

        return Ok(fabricantes);
    }

    // GET: api/fabricantes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FabricanteResponseDto>> GetById(int id)
    {
        var fabricante = await _context.Fabricantes
            .Where(f => f.Id == id)
            .Select(f => new FabricanteResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                PaisOrigem = f.PaisOrigem,
                QuantidadeVeiculos = f.Veiculos.Count
            })
            .FirstOrDefaultAsync();

        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com id {id} não encontrado." });

        return Ok(fabricante);
    }

    // POST: api/fabricantes
    [HttpPost]
    public async Task<ActionResult<FabricanteResponseDto>> Create(FabricanteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var fabricante = new Fabricante
        {
            Nome = dto.Nome,
            PaisOrigem = dto.PaisOrigem
        };

        _context.Fabricantes.Add(fabricante);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um fabricante cadastrado com esse nome." });
        }

        var response = new FabricanteResponseDto
        {
            Id = fabricante.Id,
            Nome = fabricante.Nome,
            PaisOrigem = fabricante.PaisOrigem,
            QuantidadeVeiculos = 0
        };

        return CreatedAtAction(nameof(GetById), new { id = fabricante.Id }, response);
    }

    // PUT: api/fabricantes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FabricanteUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com id {id} não encontrado." });

        fabricante.Nome = dto.Nome;
        fabricante.PaisOrigem = dto.PaisOrigem;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um fabricante cadastrado com esse nome." });
        }

        return NoContent();
    }

    // DELETE: api/fabricantes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com id {id} não encontrado." });

        _context.Fabricantes.Remove(fabricante);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem veículos cadastrados para este fabricante." });
        }

        return NoContent();
    }
}