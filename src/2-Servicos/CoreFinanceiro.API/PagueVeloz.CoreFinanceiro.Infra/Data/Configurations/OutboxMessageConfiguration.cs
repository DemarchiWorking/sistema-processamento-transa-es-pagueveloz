using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.CoreFinanceiro.Infra.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Configurations
{

    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.CreatedOnUtc).IsRequired().HasColumnName("CreatedOn");
            builder.Property(m => m.EventType).HasMaxLength(255).IsRequired();

            builder.Property(m => m.Payload)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(m => m.ProcessedOnUtc).HasColumnName("ProcessedOn");
            builder.Property(m => m.Attempts).IsRequired();
            builder.Property(m => m.NextAttemptDateUtc).IsRequired();
            builder.Property(m => m.Error).HasColumnType("text").HasMaxLength(2000); 

            builder.HasIndex(m => new { m.ProcessedOnUtc, m.NextAttemptDateUtc });
        }
    }
}