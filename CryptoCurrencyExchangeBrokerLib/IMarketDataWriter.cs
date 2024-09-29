using CryptoCurrencyExchangeBrokerLib.exchange;

namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketDataWriter
{
    void Write(AExchangeData data);
}