using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Mapeamentos
{
    public class ContaMapeamento : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.ToTable("Contas");

            builder.HasKey(c => c.AccountId);
            builder.Property(c => c.AccountId)
                .HasMaxLength(50) 
                .IsRequired();

            builder.Property(c => c.Currency)
                .HasMaxLength(3) 
                .IsRequired();

            builder.Property(c => c.SaldoDisponivelEmCentavos).IsRequired();
            builder.Property(c => c.SaldoReservadoEmCentavos).IsRequired();
            builder.Property(c => c.LimiteDeCreditoEmCentavos).IsRequired();
            builder.Property(c => c.Status)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property<byte[]>("RowVersion") 
                .IsRowVersion()
                .HasColumnName("row_version");
            builder.Ignore(c => c.DomainEvents);

        }
    }
}
