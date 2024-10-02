using CryptoCurrencyExchangeBrokerLib.exchange;

namespace BitstampLib.exchange;
internal class BitstampOrderBook : ABitstampData<OrderBook>
{
    public BitstampOrderBook(string instrument, object data)
        : base(instrument, data)
    {
    }
    public override OrderBook Load(string instrument)
    {
        return new OrderBook()
        {
            Instrument = instrument,
            Timestamp = GetMicrotimestamp(),
            Asks = GetOffers("asks"),
            Bids = GetOffers("bids")
        };
    }
}
