using Microsoft.EntityFrameworkCore;

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
