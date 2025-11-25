using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
namespace PagueVeloz.CoreFinanceiro.Infra.Data.Configurations
{
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.ToTable("Contas");

            builder.HasKey(c => c.AccountId);
            builder.Property(c => c.AccountId)
                .ValueGeneratedNever()
                .HasMaxLength(150);

            builder.Property(c => c.LockVersion)
                .IsRequired()
                .IsRowVersion(); 

            builder.HasMany(c => c.Transacoes)
                .WithOne()
                .HasForeignKey(t => t.ContaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.SaldoDisponivelEmCentavos).IsRequired();
            builder.Property(c => c.SaldoReservadoEmCentavos).IsRequired();
            builder.Property(c => c.LimiteDeCreditoEmCentavos).IsRequired();
            builder.Property(c => c.Currency).IsRequired().HasMaxLength(3);


            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Ignore(c => c.Id);

            builder.Ignore(c => c.SaldoTotal);
            builder.Ignore(c => c.PoderDeCompra);
            builder.Ignore(c => c.DomainEvents);
        }
    }
}