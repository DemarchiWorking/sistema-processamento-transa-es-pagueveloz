using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Mapeamentos
{
    public class TransacaoProcessadaMapeamento : IEntityTypeConfiguration<TransacaoProcessada>
    {
        public void Configure(EntityTypeBuilder<TransacaoProcessada> builder)
        {
            builder.ToTable("TransacoesProcessadas");
            builder.HasKey(t => t.ReferenceId);

            builder.Property(t => t.ReferenceId)
                .HasMaxLength(50) 
                .IsRequired();

            builder.Property(t => t.AccountId)
                .IsRequired();

            builder.Property(t => t.TransactionId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(t => t.MessageType)
                .HasMaxLength(100)
                .IsRequired(false); 

            builder.Property(t => t.ProcessedAt)
                .IsRequired()
                .HasColumnName("ProcessedAt")
                .HasDefaultValueSql("NOW()");

            builder.Property(t => t.Details)
                .HasColumnType("text");

            builder.HasIndex(t => t.AccountId).HasDatabaseName("IX_TransacoesProcessadas_ContaId");
        }
    }
}