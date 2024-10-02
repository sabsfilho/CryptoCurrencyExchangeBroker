using CryptoCurrencyExchangeBrokerLib.exchange;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PersistenceLayerCosmosDBLib
{
    internal class OrderBookEntity : AEntity
    {
        public OrderBook? OrderBook { get; set; }
        public OrderBookEntity() 
            : base()
        {
        }
        public OrderBookEntity(IDatabaseListener databaseListener, CredentialsMap credentialsMap, AExchangeData data)
            : base(databaseListener, credentialsMap, data)
        {
            OrderBook = (OrderBook)data;
        }

        protected override DbContext GetContext(CredentialsMap credentialsMap)
        {
            return new OrderBookContext(credentialsMap);
        }
    }
}
