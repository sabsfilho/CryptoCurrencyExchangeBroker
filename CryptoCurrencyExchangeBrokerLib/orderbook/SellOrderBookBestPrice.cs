using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal class SellOrderBookBestPrice : AOrderBookBestPrice
    {
        public SellOrderBookBestPrice(OrderBookStateInstrument orderBookStateInstrument, decimal cryptoAmount)
            : base(orderBookStateInstrument, cryptoAmount)
        {
        }
        protected override OrderBookItem[]? OrderBookItems => OrderBookStateInstrument.Bids;
    }
}
