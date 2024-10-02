using CryptoCurrencyExchangeBrokerLib;
using CryptoCurrencyExchangeBrokerLib.exchange;
using System.Drawing.Printing;

namespace PersistenceLayerCosmosDBLib
{
    public class CosmosDBWriter : IMarketDataWriter
    {
        /// <summary>
        /// Use this limit to avoid uncontrolled cloud resources costs increase
        /// </summary>
        public int WriteLimitPerSession = 0;
        private int currentWrites = 0;

        private object locker = new object();
        private IDatabaseListener databaseListener;
        private CredentialsMap credentialsMap;

        public CosmosDBWriter(
            IDatabaseListener databaseListener,
            string? connectionString = null, 
            string? databaseName = null
        )
        {
            this.databaseListener = databaseListener;

            credentialsMap = new CredentialsMap(connectionString, databaseName);
        }

        public void Write(AExchangeData data)
        {
            lock (locker)
            {
                if (WriteLimitPerSession > 0)
                {
                    currentWrites++;
                    if (currentWrites == WriteLimitPerSession)
                        return;
                }
                var entity = CreateEntity(data);

                if (entity == null)
                    return;

                entity.Persist();
            }
        }
        private AEntity? CreateEntity(AExchangeData data)
        {
            if (data is OrderBook)
                return new OrderBookEntity(databaseListener, credentialsMap, data);
            return null;
        }
    }
}
