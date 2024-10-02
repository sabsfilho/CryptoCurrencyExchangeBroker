using CryptoCurrencyExchangeBrokerLib.exchange;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal class BuyOrderBookBestPrice : AOrderBookBestPrice
    {
        public BuyOrderBookBestPrice(OrderBookState orderBookStateInstrument, decimal cryptoAmount)
            : base(orderBookStateInstrument, cryptoAmount)
        {
        }

        protected override OrderBookItem[]? OrderBookItems => OrderBookStateInstrument.Asks;

        protected override bool Buy => true;
    }
}
