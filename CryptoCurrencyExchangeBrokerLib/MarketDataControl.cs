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
}