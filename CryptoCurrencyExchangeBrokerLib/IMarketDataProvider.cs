using CryptoCurrencyExchangeBrokerLib.exchange;

namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketDataProvider
{
    string WebsocketServerEndpointUrl { get; }
    string GetSubscribeMessage(ChannelEnum channel, string instruments);
    string GetUnsubscribeMessage(ChannelEnum channel, string instruments);
    AExchangeData? MessageReceived(string msg);
}