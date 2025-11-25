using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Mapeamentos
{
    public class MovimentoMapeamento : IEntityTypeConfiguration<Movimento>
    {
        public void Configure(EntityTypeBuilder<Movimento> builder)
        {
            builder.ToTable("Movimentos");

            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Account)
                .WithMany() 
                .HasForeignKey(m => m.AccountId)
                .OnDelete(DeleteBehavior.Restrict) 
                .HasConstraintName("FK_Movimentos_ContaId");

            builder.Property(m => m.AccountId)
                .IsRequired();

            builder.Property(m => m.Tipo)
                .HasConversion<string>() 
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(m => m.Value)
                .HasColumnType("decimal(18, 4)")
                .IsRequired();

            builder.Property(m => m.Coin)
                .HasMaxLength(3) 
                .IsRequired();

            builder.Property(m => m.ReferenceId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.TransactionId)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(m => m.TransactionId).HasDatabaseName("IX_Movimentos_TransactionId");
            builder.HasIndex(m => m.ReferenceId).HasDatabaseName("IX_Movimentos_ReferenceId");


            builder.Property(m => m.Timestamp)
                .IsRequired()
            .HasDefaultValueSql("NOW()");
            builder.Property(m => m.MetadadosJson)
                .HasColumnType("text"); 
        }
    }
}