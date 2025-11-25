using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using PagueVeloz.Transferencias.Infra.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Infra.Data
{
    public class TransferenciasDbContext : SagaDbContext
    {
        public TransferenciasDbContext(DbContextOptions<TransferenciasDbContext> options)
            : base(options) { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new TransferenciaStateMap(); }
        }
    }
}