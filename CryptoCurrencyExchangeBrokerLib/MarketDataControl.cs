using CryptoCurrencyExchangeBrokerLib.exchange;
using CryptoCurrencyExchangeBrokerLib.orderbook;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib;
public class MarketDataControl : IMarketData
{
    private object locker = new object();
    private MarketDataWebSocket MarketDataWebSocket { get; set; }
    public MarketDataStatusEnum Status => MarketDataWebSocket.Status;

    public OrderBookState OrderBookState => MarketDataWebSocket.OrderBookState;
    public bool Subscribed => MarketDataWebSocket.Subscribed;

    public MarketDataControl(
        IMarketDataProvider provider,
        IMarketDataEventListener listener
    )
    {
        MarketDataWebSocket = new MarketDataWebSocket(provider, listener);
    }

    /// <summary>
    /// Connect to the websocket stream on the background thread
    /// </summary>
    public void Start()
    {
        lock (locker)
        {
            MarketDataWebSocket.Start();
        }
    }
    /// <summary>
    /// Disconnect the websocket stream 
    /// </summary>
    public void Stop()
    {
        lock (locker)
        {
            MarketDataWebSocket.Stop();
        }
    }
    /// <summary>
    /// Subscribe to a channel to feed instrument data
    /// </summary>
    /// <param name="channel">available channel</param>
    /// <param name="instrument">instrument key: btcusd or ethusd ...</param>
    public void Subscribe(ChannelEnum channel, string instrument)
    {
        lock (locker)
        {
            MarketDataWebSocket.Subscribe(channel, instrument);
        }
    }
    /// <summary>
    /// Unsubscribe to a channel to disconnect the instrument data feed
    /// </summary>
    /// <param name="channel">available channel</param>
    /// <param name="instrument">instrument key: btcusd or ethusd ...</param>
    public void Unsubscribe(ChannelEnum channel, string instrument)
    {
        lock (locker)
        {
            MarketDataWebSocket.Subscribe(channel, instrument);
        }
    }

    /// <summary>
    /// Get the Best Instrument Price to buy the Crypto amount informed
    /// </summary>
    /// <param name="buy">if true get Best Ask value, otherwise get Best Bid value</param>
    /// <param name="instrument">instrument exchange key</param>
    /// <param name="cryptoAmount">volume</param>
    /// <returns>Best Price value</returns>
    public decimal GetBestPrice(bool buy, string instrument, decimal cryptoAmount)
    {
        lock (locker)
        {
            var orderBookStateInstrument = OrderBookState.GetState(instrument);
            if (orderBookStateInstrument == null)
                return 0;

            AOrderBookBestPrice x =
                buy ?
                new BuyOrderBookBestPrice(orderBookStateInstrument, cryptoAmount) :
                new SellOrderBookBestPrice(orderBookStateInstrument, cryptoAmount);
            return x.Value;
        }
    }

    public static MarketDataControl SubscribeOrderBook(
        IMarketDataProvider provider,
        IMarketDataEventListener listener,
        string instrument
    )
    {
        var marketDataInstance = new MarketDataControl(
            provider,
            listener
        );

        marketDataInstance.Subscribe(
            CryptoCurrencyExchangeBrokerLib.ChannelEnum.OrderBook,
            instrument
        );

        return marketDataInstance;

    }
}