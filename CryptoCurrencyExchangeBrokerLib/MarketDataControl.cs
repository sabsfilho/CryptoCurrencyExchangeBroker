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
    public string Instrument => MarketDataWebSocket.Instrument;
    public OrderBookState OrderBookState => MarketDataWebSocket.OrderBookState;
    public bool Subscribed => MarketDataWebSocket.Subscribed;

    public MarketDataControl(
        string instrument,
        IMarketDataProvider provider,
        IMarketDataEventListener listener,
        IMarketDataWriter? writer = null
    )
    {
        MarketDataWebSocket = new MarketDataWebSocket(instrument, provider, listener, writer);
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
    public void Subscribe(ChannelEnum channel)
    {
        lock (locker)
        {
            MarketDataWebSocket.Subscribe(channel);
        }
    }
    /// <summary>
    /// Unsubscribe to a channel to disconnect the instrument data feed
    /// </summary>
    /// <param name="channel">available channel</param>
    /// <param name="instrument">instrument key: btcusd or ethusd ...</param>
    public void Unsubscribe(ChannelEnum channel)
    {
        lock (locker)
        {
            MarketDataWebSocket.Subscribe(channel);
        }
    }
    /// <summary>
    /// Get the Best Instrument Price to buy the Crypto amount informed
    /// </summary>
    /// <param name="buy">if true get Best Ask value, otherwise get Best Bid value</param>
    /// <param name="instrument">instrument exchange key</param>
    /// <param name="cryptoAmount">volume</param>
    /// <returns>Best Price statement</returns>
    public OrderBookBestPriceState? GetBestPrice(bool buy, decimal cryptoAmount)
    {
        lock (locker)
        {
            return
                BuildOrderBookBestPrice(buy, cryptoAmount)
                .State;
        }
    }
    private AOrderBookBestPrice BuildOrderBookBestPrice(bool buy, decimal cryptoAmount)
    {
        return
            buy ?
            new BuyOrderBookBestPrice(OrderBookState, cryptoAmount) :
            new SellOrderBookBestPrice(OrderBookState, cryptoAmount);
    }
    public static MarketDataControl SubscribeOrderBook(
        string instrument,
        IMarketDataProvider provider,
        IMarketDataEventListener listener,
        IMarketDataWriter? writer = null
    )
    {
        var marketDataInstance = new MarketDataControl(
            instrument,
            provider,
            listener,
            writer
        );

        marketDataInstance.Subscribe(
            CryptoCurrencyExchangeBrokerLib.ChannelEnum.OrderBook
        );

        return marketDataInstance;

    }
}