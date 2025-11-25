
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Persistencia; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DomainOutboxMessage = PagueVeloz.CoreFinanceiro.Infra.Persistencia.OutboxMessage;

using MTOutboxMessage = MassTransit.EntityFrameworkCoreIntegration.OutboxMessage;
using MTInboxState = MassTransit.EntityFrameworkCoreIntegration.InboxState;
using MTOutboxState = MassTransit.EntityFrameworkCoreIntegration.OutboxState;


namespace PagueVeloz.CoreFinanceiro.Infra.Data
{
    public class CoreFinanceiroContext : DbContext
    {
        private readonly IDomainEventService? _domainEventService;

        public DbSet<Conta> Contas { get; set; } = default!;
        public DbSet<Movimento> Movimentos { get; set; } = default!;
        public DbSet<TransacaoProcessada> TransacoesProcessadas { get; set; } = default!;

        public DbSet<DomainOutboxMessage> DomainOutboxMessages { get; set; } = default!;

        public DbSet<MTOutboxMessage> MtOutboxMessages { get; set; } = default!;

        public DbSet<MTOutboxState> OutboxState { get; set; } = default!;
        public DbSet<MTInboxState> InboxState { get; set; } = default!;

        public CoreFinanceiroContext(DbContextOptions<CoreFinanceiroContext> options)
            : base(options)
        {
            _domainEventService = null;
        }
        public CoreFinanceiroContext(
            DbContextOptions<CoreFinanceiroContext> options,
            IDomainEventService domainEventService)
            : base(options)
        {
            _domainEventService = domainEventService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_domainEventService != null)
            {
                await _domainEventService.PublishDomainEventsAsync();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Financeiro");

            modelBuilder.HasSequence<long>("ContaSeq", schema: "Financeiro")
                .StartsAt(1)
                .IncrementsBy(1);


            modelBuilder.AddOutboxMessageEntity(outboxMessageConfigurator =>
            {
                outboxMessageConfigurator.ToTable("MT_OutboxMessage", "Infra");
            });
            modelBuilder.AddOutboxStateEntity(outboxStateConfigurator =>
            {
                outboxStateConfigurator.ToTable("MT_OutboxState", "Infra");
            });

            modelBuilder.AddInboxStateEntity(inboxStateConfigurator =>
            {
                inboxStateConfigurator.ToTable("MT_InboxState", "Infra");
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreFinanceiroContext).Assembly);

            modelBuilder.Entity<DomainOutboxMessage>(entity =>
            {
                entity.ToTable("Domain_OutboxMessages", "Infra"); 
                entity.HasKey(m => m.Id);

                entity.HasIndex(m => m.NextAttemptDateUtc);
                entity.HasIndex(m => m.ProcessedOnUtc);

                entity.Property(m => m.Payload)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(m => m.Error)
                    .HasColumnType("text")
                    .IsRequired(false);

                entity.Property(m => m.CreatedOnUtc)
                    .IsRequired();
            });

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}