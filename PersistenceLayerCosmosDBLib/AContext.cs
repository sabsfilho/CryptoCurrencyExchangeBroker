using CryptoCurrencyExchangeBrokerLib.exchange;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayerCosmosDBLib
{
    internal abstract class AContext : DbContext
    {
        private CredentialsMap credentialsMap;
        public AContext(CredentialsMap credentialsMap)
        {
            this.credentialsMap = credentialsMap;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
               credentialsMap.ConnectionString,
               credentialsMap.DatabaseName
            );
        }

    }
}
