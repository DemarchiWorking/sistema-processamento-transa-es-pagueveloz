using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PagueVeloz.Transferencias.Dominio.Sagas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Infra.Data.Configurations
{
    public class TransferenciaStateMap : SagaClassMap<TransferenciaState>
    {
        protected override void Configure(EntityTypeBuilder<TransferenciaState> entity, ModelBuilder model)
        {
            base.Configure(entity, model);

            entity.Property(x => x.CurrentState).HasMaxLength(64);

            entity.Property(x => x.AccountIdOrigem).IsRequired().HasMaxLength(50);

            entity.Property(x => x.AccountIdDestino).IsRequired().HasMaxLength(50);

            entity.Property(x => x.Amount).IsRequired(); 

            entity.Property(x => x.ReferenceId).IsRequired().HasMaxLength(50);

            entity.Property(x => x.Currency).IsRequired().HasMaxLength(5);

            entity.Property(x => x.Timestamp).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(20); 
            entity.Property(x => x.MotivoFalha).HasMaxLength(500);

            entity.Property(x => x.DebitRequestTokenId);
            entity.Property(x => x.CreditRequestTokenId);

        }
    }
}
