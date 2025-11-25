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
    public class TransacaoProcessadaConfiguration : IEntityTypeConfiguration<TransacaoProcessada>
    {
        public void Configure(EntityTypeBuilder<TransacaoProcessada> builder)
        {
            builder.ToTable("TransacoesProcessadas");

            builder.HasKey(t => t.ReferenceId);
            builder.Property(t => t.ReferenceId).ValueGeneratedNever();
        }
    }
}