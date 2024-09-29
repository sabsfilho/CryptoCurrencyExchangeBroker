using Microsoft.IO;
using System;
using System.Net.WebSockets;
using System.Text;

namespace CryptoCurrencyExchangeBrokerLib;

internal class MarketDataWebSocket : IDisposable
{
    private const int BUFFER_SIZE = 1024 * 4;
    /* https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream */
    private static readonly RecyclableMemoryStreamManager StreamManager = new RecyclableMemoryStreamManager();

    private bool listening = false;
    private bool subscribed = false;

    private ClientWebSocket? ClientWebSocket { get; set; }
    private IMarketDataProvider Provider { get; set; }
    private IMarketDataEventListener Listener { get; set; }
    private IMarketDataWriter? Writer { get; set; }
    private CancellationTokenSource Cancellation { get; set; }
    public MarketDataStatusEnum Status { get; private set; }
    private bool CanStart =>
        Status == MarketDataStatusEnum.Undefined ||
        Status == MarketDataStatusEnum.Stopped;

    public MarketDataWebSocket(
        IMarketDataProvider provider,
        IMarketDataEventListener listener,
        IMarketDataWriter? writer = null
    )
    {
        Provider = provider;
        Listener = listener;
        Writer = writer;
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
        listening = false;
        subscribed = false;
        Cancellation = new CancellationTokenSource();

        Start();
    }

    public void Stop()
    {
        if (ClientWebSocket == null)
            return;

        Status = MarketDataStatusEnum.Stopping;

        subscribed = false;
        listening = false;

        Cancellation.Cancel();

        var task = ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
        task.Wait();
        
        Status = MarketDataStatusEnum.Stopped;
        
        Listener.ExchangeDiconnected();
    }
    public void Subscribe(ChannelEnum channel, string instrument)
    {
        if (string.IsNullOrEmpty(instrument))
            throw new MarketDataException("undefined instrument");

        if (Status != MarketDataStatusEnum.Started)
        {
            Start();
        }

        string msg = Provider.GetSubscribeMessage(channel, instrument);

        SendMessage(msg);

        subscribed = true;
    }
    public void Unsubscribe(ChannelEnum channel, string instrument)
    {
        if (string.IsNullOrEmpty(instrument))
            throw new MarketDataException("undefined instrument");

        if (Status != MarketDataStatusEnum.Started)
            throw new MarketDataException("MarketData not started!");

        subscribed = false;

        string msg = Provider.GetUnsubscribeMessage(channel, instrument);

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

            if (listening)
                throw new MarketDataException("already listening");

            Listener.MessageListenerStarting();

            var buffer = new Memory<byte>(new byte[BUFFER_SIZE]);
            do
            {
                listening = true;

                if (!subscribed) continue;

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
                    if (Writer != null && exchangeData != null)
                        Writer.Write(exchangeData);
                }

            }
            while (
                listening &&
                Status == MarketDataStatusEnum.Started &&
                !Cancellation.IsCancellationRequested
            );
        }
        catch(Exception ex)
        {
            Status = MarketDataStatusEnum.Error;
            Listener.ExceptionThrown(ex);
        }
        finally
        {
            listening = false;
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

        if (!listening)
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
