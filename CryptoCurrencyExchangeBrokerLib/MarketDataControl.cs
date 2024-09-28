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
    private IMarketDataProvider Provider { get; set; }
    private IMarketDataEventListener Listener { get; set; }
    private MarketDataWebSocket? MarketDataWebSocket { get; set; }
    public MarketDataStatusEnum Status { get; private set; }
    private bool CanStart =>
        Status == MarketDataStatusEnum.Undefined ||
        Status == MarketDataStatusEnum.Stopped;

    public MarketDataControl()
        : this(
              new provider.BitstampProvider(), 
              new LocalMarketDataEventListener()
        )
    {
    }
    public MarketDataControl(
        IMarketDataProvider provider,
        IMarketDataEventListener listener
    )
    {
        Provider = provider;
        Listener = listener;
    }

    public void Start()
    {
        lock (locker)
        {
            if (!CanStart)
                throw new MarketDataException("MarketData already started!");

            StartInternal();
        }
    }
    public void Stop()
    {
        lock (locker)
        {
            if (MarketDataWebSocket == null) 
                return;

            Status = MarketDataStatusEnum.Stopping;
            MarketDataWebSocket.Disconnect();
            Status = MarketDataStatusEnum.Stopped;
        }
    }
    public void Subscribe(ChannelEnum channel, string instrument)
    {
        lock (locker)
        {
            if (string.IsNullOrEmpty(instrument))
                throw new MarketDataException("undefined instrument");

            if (Status != MarketDataStatusEnum.Started)
            {
                StartInternal();
            }

            string msg = Provider.GetSubscribeMessage(channel, instrument);

            MarketDataWebSocket!.SendMessage(msg);
        }
    }
    public void Unsubscribe(ChannelEnum channel, string instrument)
    {
        lock (locker)
        {
            if (string.IsNullOrEmpty(instrument))
                throw new MarketDataException("undefined instrument");

            if (Status != MarketDataStatusEnum.Started)
                throw new MarketDataException("MarketData not started!");

            string msg = Provider.GetUnsubscribeMessage(channel, instrument);

            MarketDataWebSocket!.SendMessage(msg);
        }
    }
    private void StartInternal()
    {
        Status = MarketDataStatusEnum.Starting;

        MarketDataWebSocket = new MarketDataWebSocket(Provider, Listener);
        MarketDataWebSocket.Connect();

        MarketDataWebSocket.StartMessageListener();

        Status = MarketDataStatusEnum.Started;
    }

    // single instance
    private static MarketDataControl? marketDataInstance = null;
    public static MarketDataControl MarketDataInstance 
    {
        get
        {
            if (marketDataInstance == null)
            {
                marketDataInstance = new MarketDataControl();
            }
            return marketDataInstance;
        }
    }
}