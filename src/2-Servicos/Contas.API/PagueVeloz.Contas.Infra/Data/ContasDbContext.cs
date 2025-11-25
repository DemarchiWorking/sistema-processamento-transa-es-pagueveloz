using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Infra.Data
{
    public class ContasDbContext : DbContext
    {
        public DbSet<Conta> Contas { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;

        public ContasDbContext(DbContextOptions<ContasDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.HasSequence<long>("ContaSeq", schema: "Contas")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            modelBuilder.Entity<Conta>(entity =>
            {
                entity.ToTable("Contas");

                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedNever();

                entity.Property(c => c.ClienteId).IsRequired();
                entity.Property(c => c.LimiteDeCredito).IsRequired();

                entity.Property(c => c.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(c => c.LockVersion)
                    .HasColumnName("lock_version")
                    .HasColumnType("bigint")
                    .HasDefaultValue(1u)                                   
                    .ValueGeneratedOnAddOrUpdate()                       
                    .IsConcurrencyToken()
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save); 

                entity.HasOne<Cliente>()
                    .WithMany()
                    .HasForeignKey(c => c.ClienteId)
                    .IsRequired();
            });
        }
    }
}