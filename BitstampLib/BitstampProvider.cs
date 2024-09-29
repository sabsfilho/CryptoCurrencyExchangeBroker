using CryptoCurrencyExchangeBrokerLib;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BitstampLib;
/// <summary>
/// Bitstamp crypto exchange Websocket provider
/// https://www.bitstamp.net/websocket/v2/
/// </summary>
public class BitstampProvider : IMarketDataProvider
{
    public string WebsocketServerEndpointUrl => "wss://ws.bitstamp.net/";

    /// <summary>
    /// Get the subscribe instruction message text to send to Websocket stream server
    /// </summary>
    /// <param name="channel">available channel</param>
    /// <param name="instrument">instrument key: btcusd or ethusd ...</param>
    /// <returns>json text message</returns>
    public string GetSubscribeMessage(ChannelEnum channel, string instrument)
    {
        var m = GetMessage("bts:subscribe", channel, instrument);

        return
            JsonSerializer.Serialize(m);
    }

    /// <summary>
    /// Get the unsubscribe instruction message text to send to Websocket stream server
    /// </summary>
    /// <param name="channel">available channel</param>
    /// <param name="instrument">instrument key: btcusd or ethusd ...</param>
    /// <returns>json text message</returns>
    public string GetUnsubscribeMessage(ChannelEnum channel, string instrument)
    {
        var m = GetMessage("bts:unsubscribe", channel, instrument);

        return
            JsonSerializer.Serialize(m);
    }

    private BitstampRequestMessage GetMessage(string eventName, ChannelEnum channel, string instrument)
    {
        return
            new BitstampRequestMessage()
            {
                Event = eventName,
                Data =
                    new BitstampRequestData()
                    {
                        Channel = $"{GetChannelTag(channel)}_{instrument}"
                    }
            };
    }

    public void MessageReceived(string msg)
    {
        var o = JsonSerializer.Deserialize<BitstampResponseMessage>(msg)!;
        if (o.Event == "bts:error")
            throw new Exception(o.Data == null ? "error" : o.Data.Message ?? "error");
        
        
    }

    private string GetChannelTag(ChannelEnum channel)
    {
        switch (channel)
        {
            case ChannelEnum.DetailOrderBook:
                return "detail_order_book";
            case ChannelEnum.FullOrderBook:
                return "diff_order_book";
            case ChannelEnum.OrderBook:
                return "order_book";
            case ChannelEnum.Order:
                return "live_orders_book";
            case ChannelEnum.Ticker:
                return "live_trades_book";
        }

        throw new MarketDataException("undefined channel");
    }
}
