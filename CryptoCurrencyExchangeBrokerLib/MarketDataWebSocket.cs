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

    private bool running = false;
    private bool listening = false;
    private ClientWebSocket? ClientWebSocket { get; set; }
    private IMarketDataProvider Provider { get; set; }
    private IMarketDataEventListener Listener { get; set; }
    private CancellationTokenSource Cancellation { get; set; }

    public MarketDataWebSocket(
        IMarketDataProvider provider,
        IMarketDataEventListener listener
    )
    {
        Provider = provider;
        Listener = listener;
    }

    /// <summary>
    /// Connect to the websocket stream on the background thread
    /// </summary>
    public async void Connect()
    {
        Cancellation = new CancellationTokenSource();
        string url = Provider.WebsocketServerEndpointUrl;
        var uri = new Uri(url);
        ClientWebSocket = new ClientWebSocket();
        await ClientWebSocket.ConnectAsync(uri, Cancellation.Token);
        running = true;
        Listener.ExchangeConnected(url);
    }
    /// <summary>
    /// Disconnect the websocket stream 
    /// </summary>
    public async void Disconnect()
    {
        if (ClientWebSocket == null)
            return;

        Cancellation.Cancel();

        await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
        Listener.ExchangeDiconnected();
    }
    /// <summary>
    /// Start receiving messages from WebSocket stream
    /// </summary>
    /// <exception cref="MarketDataException"></exception>
    public async void StartMessageListener()
    {
        if (ClientWebSocket == null)
            throw new MarketDataException("Websocket disconnected");

        Listener.MessageListenerStarting();

        var buffer = new Memory<byte>(new byte[BUFFER_SIZE]);
        do
        {
            using (var ms = StreamManager.GetStream())
            {

                while (true)
                {
                    listening = true;

                    var r = await ClientWebSocket.ReceiveAsync(buffer, Cancellation.Token);
                    ms.Write(buffer[..r.Count].Span);

                    if (r.EndOfMessage)
                        break;
                }

                ms.Seek(0, SeekOrigin.Begin);

                var msg = Encoding.UTF8.GetString(ms.ToArray());

                Listener.MessageReceived(msg);
            }

        }
        while (
            running && 
            !Cancellation.IsCancellationRequested
        );


        listening = false;

        Listener.MessageListenerFinished();
    }
    /// <summary>
    /// Send message to WebSocket stream
    /// </summary>
    /// <exception cref="MarketDataException"></exception>
    public async void SendMessage(string msg)
    {
        if (ClientWebSocket == null)
            throw new MarketDataException("Websocket disconnected");

        if (!listening)
        {
            StartMessageListener();
        }

        var buffer = Encoding.UTF8.GetBytes(msg);
        await ClientWebSocket.SendAsync(
            new ArraySegment<byte>(buffer), 
            WebSocketMessageType.Text, 
            true, 
            Cancellation.Token
        );

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
