namespace CryptoCurrencyExchangeBrokerLib;
public class MarketDataException : Exception
{
    public MarketDataException(string msg)
        : base(msg)
    {
    }
}
