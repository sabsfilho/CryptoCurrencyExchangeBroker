using BitstampLib;
using CryptoCurrencyExchangeBrokerLib;
using CryptoCurrencyExchangeBrokerLib.exchange;
using CryptoCurrencyExchangeBrokerLib.orderbook;
using PersistenceLayerCosmosDBLib;

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

        public int ForceStopInMilliseconds { get; set; }
        public bool DatabaseEnabled { get; set; }
        public int WriteLimitPerSession { get; set; }

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

                if (ForceStopInMilliseconds > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(ForceStopInMilliseconds);
                        Stop();
                    });
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

                MarketDataInstances = null;
            }
        }

        internal void SubscribeOrderBook(string instrument)
        {
            lock (locker)
            {
                if (MarketDataInstances == null)
                    Start(null);

                CosmosDBWriter? databaseWriter = null;
                if (DatabaseEnabled)
                {
                    databaseWriter = new CosmosDBWriter(cryptoHandlerListener)
                    {
                        WriteLimitPerSession = WriteLimitPerSession
                    };
                }

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

        internal OrderBook[] GetOrderBookStateFromCosmosDB(string instrument)
        {
            if (!DatabaseEnabled)
                throw new MarketDataException("Database is disable to reduce costs. Enable it first before starting the app.");

            return new OrderBook[0];
        }
    }
}
