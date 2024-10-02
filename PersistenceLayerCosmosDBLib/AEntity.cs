using CryptoCurrencyExchangeBrokerLib.exchange;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayerCosmosDBLib
{
    internal abstract class AEntity
    {
        public string ID { get; set; }
        public DateTime Creation { get; set; }
        protected abstract DbContext GetContext(CredentialsMap credentialsMap);

        private IDatabaseListener? databaseListener;
        private CredentialsMap? credentialsMap;
        protected AEntity()
        {
            ID = Guid.NewGuid().ToString();
            Creation = DateTime.Now;
        }

        protected AEntity(
            IDatabaseListener databaseListener,
            CredentialsMap credentialsMap,
            AExchangeData data
        ) : this()
        {
            this.databaseListener = databaseListener;
            this.credentialsMap = credentialsMap;
        }

        /// <summary>
        /// Default behavior just writes the object instance data to the database.
        /// You can override it to give another behavior.
        /// </summary>
        internal virtual void Persist()
        {
            using (var context = GetContext(credentialsMap!))
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(this);
                context.SaveChangesFailed += Context_SaveChangesFailed;
                context.SaveChanges();
                Thread.Sleep(10);
            }
        }

        private void Context_SaveChangesFailed(object? sender, SaveChangesFailedEventArgs e)
        {
            if (databaseListener != null)
                databaseListener.SaveChangesFailed(e.Exception);
        }
    }
}
