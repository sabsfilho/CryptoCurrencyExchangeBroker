using System.Text.Json;

namespace CryptoCurrencyExchangeBrokerLib.provider;
public class BitstampProvider : IMarketDataProvider
{
    public string WebsocketServerEndpointUrl => "wss://ws.bitstamp.net/";

    public string GetSubscribeMessage(ChannelEnum channel, string instrument)
    {
        var m = GetMessage("bts:subscribe", channel, instrument);

        return
            JsonSerializer.Serialize(m);
    }

    public string GetUnsubscribeMessage(ChannelEnum channel, string instrument)
    {
        var m = GetMessage("bts:unsubscribe", channel, instrument);

        return
            JsonSerializer.Serialize(m);
    }

    private string GetMessage(string eventName, ChannelEnum channel, string instrument)
    {
        var m =
            new BitstampMessage()
            {
                Event = eventName,
                Data =
                    new BitstampData()
                    {
                        Channel = $"{GetChannelTag(channel)}_{instrument}"
                    }
            };

        return
            JsonSerializer.Serialize(m);
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
