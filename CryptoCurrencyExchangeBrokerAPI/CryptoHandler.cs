using BitstampLib;
using CryptoCurrencyExchangeBrokerLib;
using CryptoCurrencyExchangeBrokerLib.exchange;
using CryptoCurrencyExchangeBrokerLib.orderbook;
using PersistenceLayerCosmosDBLib;
using System.Diagnostics.Metrics;

namespace CryptoCurrencyExchangeBrokerAPI
{
    public class CryptoHandler
    {
        private static string[] DEFAULT_INSTRUMENTS = 
            new string[]
            {
                "btcusd",
                "ethusd"
            };

        private object locker = new object();
        private CryptoHandlerListener cryptoHandlerListener;

        private Dictionary<string, MarketDataControl>? MarketDataInstances { get; set; }
        private BitstampProvider? Provider { get; set; }

        public CryptoHandler(CryptoHandlerListener cryptoHandlerListener)
        {
            this.cryptoHandlerListener = cryptoHandlerListener;
        }

        public void Start()
        {
            Start(DEFAULT_INSTRUMENTS);
        }
        public void Start(string[]? instruments)
        {
            lock (locker)
            {
                MarketDataInstances = new Dictionary<string, MarketDataControl>();

                Provider = new BitstampProvider();

                if (instruments != null)
                {
                    foreach (var instrument in instruments)
                    {
                        SubscribeOrderBook(instrument);
                    }
                }
            }
        }
        public void Stop()
        {
            lock (locker)
            {
                if (MarketDataInstances == null)
                    return;

                foreach (var item in MarketDataInstances)
                {
                    item.Value.Stop();
                }
            }
        }

        internal void SubscribeOrderBook(string instrument)
        {
            lock (locker)
            {
                if (MarketDataInstances == null)
                    Start(null);


                var databaseWriter = new CosmosDBWriter(cryptoHandlerListener)
                {
                    WriteLimitPerSession = 10
                };

                var marketDataInstance =
                    MarketDataControl.SubscribeOrderBook(
                        instrument,
                        Provider!,
                        cryptoHandlerListener,
                        databaseWriter
                    );

                MarketDataInstances!.Add(instrument, marketDataInstance);
            }
        }

        internal void UnsubscribeOrderBook(string instrument)
        {
            lock (locker)
            {
                if (
                    MarketDataInstances == null ||
                    !MarketDataInstances.ContainsKey(instrument)
                )
                {
                    return;
                }
                MarketDataInstances[instrument].Stop();
            }
        }

        internal OrderBookState? GetOrderBookState(string instrument)
        {
            lock (locker)
            {
                if (
                    MarketDataInstances == null ||
                    !MarketDataInstances.ContainsKey(instrument)
                )
                {
                    return null;
                }

                return
                    MarketDataInstances[instrument]
                    .OrderBookState;
            }
        }

        internal OrderBookBestPriceState? GetBestPrice(string instrument, bool buy, decimal cryptoAmount)
        {
            lock (locker)
            {
                if (
                    MarketDataInstances == null ||
                    !MarketDataInstances.ContainsKey(instrument)
                )
                {
                    return null;
                }

                return
                    MarketDataInstances[instrument]
                    .GetBestPrice(buy, cryptoAmount);
            }
        }

        internal void AutoStop(int delayInMilliseconds)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(delayInMilliseconds);
                Stop();
            });
        }

        internal OrderBook[] GetOrderBookStateFromCosmosDB(string instrument)
        {
            return new OrderBook[0];
        }
    }
}
