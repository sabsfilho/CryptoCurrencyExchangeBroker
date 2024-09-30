using BitstampLib.exchange;
using CryptoCurrencyExchangeBrokerLib;
using CryptoCurrencyExchangeBrokerLib.exchange;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Channels;

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

    public AExchangeData? MessageReceived(string msg)
    {
        var o = JsonSerializer.Deserialize<BitstampResponseMessage>(msg)!;

        if (o.Event == "bts:error")
        {
            var responseData = o.Data as BitstampResponseData;

            string m =
                responseData == null ? "error" :
                responseData.Message ?? "error";
            throw new BitstampException(m);
        }

        if (o.Event != "data")
            return null;

        return
            GetExchangeData(o);
    }

    private AExchangeData GetExchangeData(BitstampResponseMessage o)
    {
        if (o.Channel == null)
            throw new BitstampException("channel is undefined");

        if (o.Data == null)
            throw new BitstampException("data is undefined");

        string channel = o.Channel;
        var x = GetChannelAndInstrument(channel);
        switch (x.Channel)
        {
            case "order_book":
                return new BitstampOrderBook(x.Instrument, o.Data).ExchangeData;
            case "detail_order_book":
            case "diff_order_book":
            case "live_orders_book":
            case "live_trades_book":
                throw new NotImplementedException(x.Channel);
        }

        throw new BitstampException($"undefined channel {x.Channel}");

    }

    private static (string Channel, string Instrument) GetChannelAndInstrument(string channel)
    {
        int i = channel.LastIndexOf('_');
        if (i == -1)
            throw new BitstampException($"instrument undefined in {channel}");
        return (
            channel.Substring(0, i),
            channel.Substring(i+1)
        );
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

        throw new BitstampException($"undefined channel {channel.ToString()}");
    }
}
