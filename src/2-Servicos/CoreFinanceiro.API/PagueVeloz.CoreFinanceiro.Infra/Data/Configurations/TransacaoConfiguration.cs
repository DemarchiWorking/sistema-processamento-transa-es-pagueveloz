using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Configurations
{
    public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("Transacoes");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();

            builder.Property(t => t.ContaId).IsRequired().HasMaxLength(50);

            builder.HasIndex(t => new { t.ContaId, t.ReferenceId }).IsUnique();

            builder.Property(t => t.Tipo)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(t => t.Valor).IsRequired();
            builder.Property(t => t.ReferenceId).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Currency).IsRequired().HasMaxLength(3);
            builder.Property(t => t.Timestamp).IsRequired();

            builder.Property(t => t.IsReversed).IsRequired();
            builder.Property(t => t.ReversalReferenceId).HasMaxLength(100);

            builder.HasIndex(t => t.ContaId);
            builder.HasIndex(t => t.ReferenceId); 
        }
    }
}