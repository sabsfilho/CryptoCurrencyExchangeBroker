﻿using CryptoCurrencyExchangeBrokerLib.orderbook;
using Microsoft.IO;
using System.Net.WebSockets;
using System.Text;

namespace CryptoCurrencyExchangeBrokerLib;

internal class MarketDataWebSocket : IDisposable
{
    private const int BUFFER_SIZE = 1024 * 4;
    /* https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream */
    private static readonly RecyclableMemoryStreamManager StreamManager = new RecyclableMemoryStreamManager();
    public string Instrument { get; private set; }
    public MarketDataStatusEnum Status { get; private set; }
    public OrderBookState OrderBookState { get; set; }
    public bool Subscribed { get; private set; }

    private bool Listening { get; set; }
    private ClientWebSocket? ClientWebSocket { get; set; }
    private IMarketDataProvider Provider { get; set; }
    private IMarketDataEventListener Listener { get; set; }
    private IMarketDataWriter? Writer { get; set; }
    private CancellationTokenSource Cancellation { get; set; }
    private bool CanStart =>
        Status == MarketDataStatusEnum.Undefined ||
        Status == MarketDataStatusEnum.Stopped;

    public MarketDataWebSocket(
        string instrument,
        IMarketDataProvider provider,
        IMarketDataEventListener listener,
        IMarketDataWriter? writer = null
    )
    {
        if (string.IsNullOrEmpty(instrument))
            throw new MarketDataException("undefined instrument");

        Instrument = instrument;
        Provider = provider;
        Listener = listener;
        Writer = writer;

        OrderBookState = new OrderBookState(instrument);

        Status = MarketDataStatusEnum.Undefined;

        Cancellation = new CancellationTokenSource();
    }

    public void Start()
    {
        if (!CanStart)
            throw new MarketDataException("MarketData already started!");

        Status = MarketDataStatusEnum.Starting;

        string url = Provider.WebsocketServerEndpointUrl;
        var uri = new Uri(url);
        ClientWebSocket = new ClientWebSocket();
        var task = ClientWebSocket.ConnectAsync(uri, Cancellation.Token);
        task.Wait();

        Status = MarketDataStatusEnum.Started;

        Listener.ExchangeConnected(url);
    }

    public void Restart()
    {
        Status = MarketDataStatusEnum.Restarting;
        Listener.MessageListenerRestarting();
        ClientWebSocket = null;
        Status = MarketDataStatusEnum.Undefined;
        Listening = false;
        Subscribed = false;
        Cancellation = new CancellationTokenSource();

        Start();
    }

    public void Stop()
    {
        if (ClientWebSocket == null)
            return;

        Status = MarketDataStatusEnum.Stopping;

        Subscribed = false;
        Listening = false;

        Cancellation.Cancel();

        if (ClientWebSocket.State == WebSocketState.Open)
        {
            var task = ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
            task.Wait();
        }

        Status = MarketDataStatusEnum.Stopped;

        Listener.ExchangeDiconnected();
    }
    public void Subscribe(ChannelEnum channel)
    {

        if (Status != MarketDataStatusEnum.Started)
        {
            Start();
        }

        string msg = Provider.GetSubscribeMessage(channel, Instrument);

        SendMessage(msg);

        Subscribed = true;
    }
    public void Unsubscribe(ChannelEnum channel)
    {
        if (Status != MarketDataStatusEnum.Started)
            throw new MarketDataException("MarketData not started!");

        Subscribed = false;

        string msg = Provider.GetUnsubscribeMessage(channel, Instrument);

        SendMessage(msg);
    }
    /// <summary>
    /// Start receiving messages from WebSocket stream
    /// </summary>
    /// <exception cref="MarketDataException"></exception>
    private async Task StartMessageListener()
    {
        try
        {
            if (ClientWebSocket == null)
                throw new MarketDataException("Websocket disconnected");

            if (Listening)
                throw new MarketDataException("already listening");

            Listener.MessageListenerRunning();

            var buffer = new Memory<byte>(new byte[BUFFER_SIZE]);
            do
            {
                Listening = true;

                if (!Subscribed) continue;

                using (var ms = StreamManager.GetStream())
                {
                    while (true)
                    {
                        var r = await ClientWebSocket.ReceiveAsync(buffer, Cancellation.Token);
                        ms.Write(buffer[..r.Count].Span);

                        if (r.EndOfMessage)
                            break;
                    }

                    ArraySegment<byte> buf;
                    ms.TryGetBuffer(out buf);
                    var msg = Encoding.UTF8.GetString(buf.ToArray());

                    Listener.MessageReceived(msg);
                    var exchangeData = Provider.MessageReceived(msg);
                    OrderBookState.MessageReceived(exchangeData);
                    if (Writer != null && exchangeData != null)
                        Writer.Write(exchangeData);
                }

            }
            while (
                Listening &&
                Status == MarketDataStatusEnum.Started &&
                !Cancellation.IsCancellationRequested
            );
        }
        catch (Exception ex)
        {
            Status = MarketDataStatusEnum.Error;
            Listener.ExceptionThrown(ex);
        }
        finally
        {
            Listening = false;
        }

        Listener.MessageListenerFinished();
    }
    /// <summary>
    /// Send message to WebSocket stream
    /// </summary>
    /// <exception cref="MarketDataException"></exception>
    private void SendMessage(string msg)
    {
        if (ClientWebSocket == null)
            throw new MarketDataException("Websocket disconnected");

        if (!Listening)
        {
            Task.Factory.StartNew(StartMessageListener);
        }

        Listener.SendMessage(msg);

        var buffer = Encoding.UTF8.GetBytes(msg);
        var task = ClientWebSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            Cancellation.Token
        );
        task.Wait();
    }

    /// <summary>
    /// clean up
    /// </summary>
    public void Dispose()
    {
        if (ClientWebSocket != null)
        {
            Cancellation.Cancel();
            ClientWebSocket.Abort();
            ClientWebSocket.Dispose();
            Cancellation.Dispose();
        }

    }
}
