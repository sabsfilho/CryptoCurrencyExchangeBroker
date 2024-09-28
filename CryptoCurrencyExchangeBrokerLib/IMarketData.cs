namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketData
{
    MarketDataStatusEnum Status { get; }
    void Start();
    void Stop();
    void Subscribe(ChannelEnum channel, string instrument);
    void Unsubscribe(ChannelEnum channe, string instrumentl);
}