using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly ApplicationContext _context;

    public VeiculosController(ApplicationContext context)
    {
        _context = context;
    }

    // GET: api/veiculos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VeiculoResponseDto>>> GetAll()
    {
        var veiculos = await ProjetarVeiculos(_context.Veiculos).ToListAsync();
        return Ok(veiculos);
    }

    // GET: api/veiculos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VeiculoResponseDto>> GetById(int id)
    {
        var veiculo = await ProjetarVeiculos(_context.Veiculos.Where(v => v.Id == id)).FirstOrDefaultAsync();

        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com id {id} não encontrado." });

        return Ok(veiculo);
    }

    // FILTRO 1: veículos disponíveis, opcionalmente por categoria.
    // Usa Include (Fabricante e Categoria) -> o EF Core gera LEFT JOIN no SQL.
    [HttpGet("disponiveis")]
    public async Task<ActionResult<IEnumerable<VeiculoResponseDto>>> GetDisponiveis([FromQuery] int? categoriaId)
    {
        var query = _context.Veiculos
            .Include(v => v.Fabricante)
            .Include(v => v.Categoria)
            .Where(v => v.Status == StatusVeiculo.Disponivel);

        if (categoriaId.HasValue)
            query = query.Where(v => v.CategoriaId == categoriaId.Value);

        var veiculos = await query
            .Select(v => new VeiculoResponseDto
            {
                Id = v.Id,
                Modelo = v.Modelo,
                AnoFabricacao = v.AnoFabricacao,
                Quilometragem = v.Quilometragem,
                Placa = v.Placa,
                Cor = v.Cor,
                Status = v.Status,
                FabricanteId = v.FabricanteId,
                FabricanteNome = v.Fabricante.Nome,
                CategoriaId = v.CategoriaId,
                CategoriaNome = v.Categoria.Nome
            })
            .ToListAsync();

        return Ok(veiculos);
    }

    // FILTRO 2: veículos de um fabricante específico.
    // Usa "join" explícito do LINQ com duas tabelas -> o EF Core gera INNER JOIN no SQL.
    [HttpGet("por-fabricante/{fabricanteId:int}")]
    public async Task<ActionResult<IEnumerable<VeiculoResponseDto>>> GetPorFabricante(int fabricanteId)
    {
        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == fabricanteId);
        if (!fabricanteExiste)
            return NotFound(new { mensagem = $"Fabricante com id {fabricanteId} não encontrado." });

        var veiculos = await (
            from v in _context.Veiculos
            join f in _context.Fabricantes on v.FabricanteId equals f.Id
            join c in _context.Categorias on v.CategoriaId equals c.Id
            where f.Id == fabricanteId
            select new VeiculoResponseDto
            {
                Id = v.Id,
                Modelo = v.Modelo,
                AnoFabricacao = v.AnoFabricacao,
                Quilometragem = v.Quilometragem,
                Placa = v.Placa,
                Cor = v.Cor,
                Status = v.Status,
                FabricanteId = f.Id,
                FabricanteNome = f.Nome,
                CategoriaId = c.Id,
                CategoriaNome = c.Nome
            }
        ).ToListAsync();

        return Ok(veiculos);
    }

    // POST: api/veiculos
    [HttpPost]
    public async Task<ActionResult<VeiculoResponseDto>> Create(VeiculoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == dto.FabricanteId);
        if (!fabricanteExiste)
            return BadRequest(new { mensagem = $"Fabricante com id {dto.FabricanteId} não existe." });

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return BadRequest(new { mensagem = $"Categoria com id {dto.CategoriaId} não existe." });

        var veiculo = new Veiculo
        {
            Modelo = dto.Modelo,
            AnoFabricacao = dto.AnoFabricacao,
            Quilometragem = dto.Quilometragem,
            Placa = dto.Placa,
            Cor = dto.Cor,
            FabricanteId = dto.FabricanteId,
            CategoriaId = dto.CategoriaId,
            Status = StatusVeiculo.Disponivel
        };

        _context.Veiculos.Add(veiculo);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um veículo cadastrado com essa placa." });
        }

        var response = await ProjetarVeiculos(_context.Veiculos.Where(v => v.Id == veiculo.Id)).FirstAsync();
        return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, response);
    }

    // PUT: api/veiculos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VeiculoUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com id {id} não encontrado." });

        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == dto.FabricanteId);
        if (!fabricanteExiste)
            return BadRequest(new { mensagem = $"Fabricante com id {dto.FabricanteId} não existe." });

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return BadRequest(new { mensagem = $"Categoria com id {dto.CategoriaId} não existe." });

        veiculo.Modelo = dto.Modelo;
        veiculo.AnoFabricacao = dto.AnoFabricacao;
        veiculo.Quilometragem = dto.Quilometragem;
        veiculo.Placa = dto.Placa;
        veiculo.Cor = dto.Cor;
        veiculo.FabricanteId = dto.FabricanteId;
        veiculo.CategoriaId = dto.CategoriaId;
        veiculo.Status = dto.Status;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um veículo cadastrado com essa placa." });
        }

        return NoContent();
    }

    // DELETE: api/veiculos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com id {id} não encontrado." });

        _context.Veiculos.Remove(veiculo);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: este veículo possui aluguéis registrados." });
        }

        return NoContent();
    }

    private static IQueryable<VeiculoResponseDto> ProjetarVeiculos(IQueryable<Veiculo> query)
    {
        return query
            .Include(v => v.Fabricante)
            .Include(v => v.Categoria)
            .Select(v => new VeiculoResponseDto
            {
                Id = v.Id,
                Modelo = v.Modelo,
                AnoFabricacao = v.AnoFabricacao,
                Quilometragem = v.Quilometragem,
                Placa = v.Placa,
                Cor = v.Cor,
                Status = v.Status,
                FabricanteId = v.FabricanteId,
                FabricanteNome = v.Fabricante.Nome,
                CategoriaId = v.CategoriaId,
                CategoriaNome = v.Categoria.Nome
            });
    }
}