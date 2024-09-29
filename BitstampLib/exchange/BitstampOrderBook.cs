using CryptoCurrencyExchangeBrokerLib.exchange;
using System.Text.Json.Serialization;

namespace BitstampLib.exchange;
internal class BitstampOrderBook : ABitstampData<OrderBook>
{
    public BitstampOrderBook(string ticker, object data)
        : base(ticker, data)
    {
    }
    public override OrderBook Load(string ticker)
    {
        return new OrderBook() 
        {
            Ticker = ticker,
            Timestamp = GetMicrotimestamp(),
            Asks = GetOffers("asks"),
            Bids = GetOffers("bids")
        };
    }
}
