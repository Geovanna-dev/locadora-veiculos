using Locadora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Aluguel> Alugueis => Set<Aluguel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- Fabricante ----------
        modelBuilder.Entity<Fabricante>(e =>
        {
            e.HasIndex(f => f.Nome).IsUnique();
        });

        // ---------- Categoria ----------
        modelBuilder.Entity<Categoria>(e =>
        {
            e.HasIndex(c => c.Nome).IsUnique();

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Categoria_ValorDiariaBase", "[ValorDiariaBase] > 0");
            });
        });

        // ---------- Veiculo ----------
        modelBuilder.Entity<Veiculo>(e =>
        {
            e.HasIndex(v => v.Placa).IsUnique();

            // Guarda o status como texto ("Disponivel", "Alugado"...) para facilitar a leitura no banco
            e.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);

            e.HasOne(v => v.Fabricante)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(v => v.Categoria)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Veiculo_AnoFabricacao", "[AnoFabricacao] >= 1900");
                t.HasCheckConstraint("CK_Veiculo_Quilometragem", "[Quilometragem] >= 0");
            });
        });

        // ---------- Cliente ----------
        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasIndex(c => c.Cpf).IsUnique();
            e.HasIndex(c => c.Email).IsUnique();
        });

        // ---------- Aluguel ----------
        modelBuilder.Entity<Aluguel>(e =>
        {
            e.HasOne(a => a.Cliente)
                .WithMany(c => c.Alugueis)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Veiculo)
                .WithMany(v => v.Alugueis)
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Um veículo só pode ter UM aluguel em aberto (sem devolução) por vez
            e.HasIndex(a => a.VeiculoId)
                .IsUnique()
                .HasFilter("[DataDevolucao] IS NULL")
                .HasDatabaseName("UX_Aluguel_Veiculo_EmAberto");

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Aluguel_PeriodoPrevisto",
                    "[DataPrevistaDevolucao] >= [DataRetirada]");

                t.HasCheckConstraint("CK_Aluguel_DataDevolucao",
                    "[DataDevolucao] IS NULL OR [DataDevolucao] >= [DataRetirada]");

                t.HasCheckConstraint("CK_Aluguel_QuilometragemInicial",
                    "[QuilometragemInicial] >= 0");

                t.HasCheckConstraint("CK_Aluguel_QuilometragemFinal",
                    "[QuilometragemFinal] IS NULL OR [QuilometragemFinal] >= [QuilometragemInicial]");

                t.HasCheckConstraint("CK_Aluguel_ValorDiaria", "[ValorDiaria] > 0");

                t.HasCheckConstraint("CK_Aluguel_ValorTotal",
                    "[ValorTotal] IS NULL OR [ValorTotal] >= 0");

                // A devolução é registrada "de uma vez": data, km final e valor total juntos
                t.HasCheckConstraint("CK_Aluguel_DevolucaoCompleta",
                    "([DataDevolucao] IS NULL AND [QuilometragemFinal] IS NULL AND [ValorTotal] IS NULL) " +
                    "OR ([DataDevolucao] IS NOT NULL AND [QuilometragemFinal] IS NOT NULL AND [ValorTotal] IS NOT NULL)");
            });
        });
    }
}