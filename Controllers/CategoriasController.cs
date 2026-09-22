using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationContext _context;

    public CategoriasController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetAll()
    {
        var categorias = await _context.Categorias
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                ValorDiariaBase = c.ValorDiariaBase
            })
            .ToListAsync();

        return Ok(categorias);
    }

    // GET: api/categorias/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaResponseDto>> GetById(int id)
    {
        var categoria = await _context.Categorias
            .Where(c => c.Id == id)
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                ValorDiariaBase = c.ValorDiariaBase
            })
            .FirstOrDefaultAsync();

        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com id {id} não encontrada." });

        return Ok(categoria);
    }

    // POST: api/categorias
    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> Create(CategoriaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var categoria = new Categoria
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            ValorDiariaBase = dto.ValorDiariaBase
        };

        _context.Categorias.Add(categoria);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe uma categoria cadastrada com esse nome." });
        }

        var response = new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            ValorDiariaBase = categoria.ValorDiariaBase
        };

        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, response);
    }

    // PUT: api/categorias/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoriaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com id {id} não encontrada." });

        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;
        categoria.ValorDiariaBase = dto.ValorDiariaBase;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe uma categoria cadastrada com esse nome." });
        }

        return NoContent();
    }

    // DELETE: api/categorias/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com id {id} não encontrada." });

        _context.Categorias.Remove(categoria);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem veículos cadastrados nesta categoria." });
        }

        return NoContent();
    }
}