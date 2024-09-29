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
    private CancellationTokenSource Cancellation { get; set; }
    public MarketDataStatusEnum Status { get; private set; }
    private bool CanStart =>
        Status == MarketDataStatusEnum.Undefined ||
        Status == MarketDataStatusEnum.Stopped;

    public MarketDataWebSocket(
        IMarketDataProvider provider,
        IMarketDataEventListener listener
    )
    {
        Provider = provider;
        Listener = listener;
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
        if (ClientWebSocket == null)
            throw new MarketDataException("Websocket disconnected");

        if (listening)
            throw new MarketDataException("already listening");

        try
        {
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

                    ms.Seek(0, SeekOrigin.Begin);

                    var msg = Encoding.UTF8.GetString(ms.ToArray());

                    Listener.MessageReceived(msg);
                    Provider.MessageReceived(msg);
                }

            }
            while (
                listening &&
                Status == MarketDataStatusEnum.Started &&
                !Cancellation.IsCancellationRequested
            );
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
