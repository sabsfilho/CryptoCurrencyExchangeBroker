namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketData
{
    MarketDataStatusEnum Status { get; }
    void Start();
    void Stop();
    void Subscribe(ChannelEnum channel, string instrument);
    void Unsubscribe(ChannelEnum channel, string instrument);
    decimal GetBestPrice(bool buy, string instrument, decimal cryptoAmount);
}