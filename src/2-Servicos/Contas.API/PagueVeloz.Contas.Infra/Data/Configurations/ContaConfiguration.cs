using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Infra.Data.Configurations
{
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.ToTable("Contas");

            builder.HasKey(c => c.Id);

            //alert:id nao gerado pelo banco
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.ClienteId).IsRequired();
            builder.Property(c => c.LimiteDeCredito).IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            //alert:relacionamento verificar no desafio
            // builder.HasOne<Cliente>()
            //     .WithMany()
            //     .HasForeignKey(c => c.ClienteId);
        }
    }
}