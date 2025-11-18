using MassTransit;
using Microsoft.EntityFrameworkCore;
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
        //agregados dominio
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public ContasDbContext(DbContextOptions<ContasDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //faz a aplicacao das configuracoes de Entitytype
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //adiciona as tabelas necessárias para [outbox do massttransit] | alert:transacao atomica.
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}