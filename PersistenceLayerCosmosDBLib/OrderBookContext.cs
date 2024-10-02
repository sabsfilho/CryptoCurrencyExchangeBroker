using CryptoCurrencyExchangeBrokerLib.exchange;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersistenceLayerCosmosDBLib
{
    internal class OrderBookContext : AContext
    {
        public DbSet<OrderBookEntity> OrderBookItems { get; set; }
        public OrderBookContext(CredentialsMap credentialsMap) 
            : base(credentialsMap)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderBookEntity>()
                .ToContainer("OrderBook");

            modelBuilder.Entity<OrderBookEntity>()
                .HasNoDiscriminator();

            modelBuilder.Entity<OrderBookEntity>()
                .HasPartitionKey(o => o.ID);

            modelBuilder.Entity<OrderBookEntity>()
                .UseETagConcurrency();

            modelBuilder.Entity<OrderBookEntity>()
                .Property<OrderBook?>(o => o.OrderBook)            
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<OrderBook?>(v, (JsonSerializerOptions?)null));
        }
    }
}
