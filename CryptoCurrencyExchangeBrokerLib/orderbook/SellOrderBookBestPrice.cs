using CryptoCurrencyExchangeBrokerLib.exchange;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal class SellOrderBookBestPrice : AOrderBookBestPrice
    {
        public SellOrderBookBestPrice(OrderBookState orderBookStateInstrument, decimal cryptoAmount)
            : base(orderBookStateInstrument, cryptoAmount)
        {
        }
        protected override OrderBookItem[]? OrderBookItems => OrderBookStateInstrument.Bids;
        protected override bool Buy => false;
    }
}
