using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal class BuyOrderBookBestPrice : AOrderBookBestPrice
    {
        public BuyOrderBookBestPrice(OrderBookStateInstrument orderBookStateInstrument, decimal cryptoAmount)
            : base(orderBookStateInstrument, cryptoAmount)
        {
        }

        protected override OrderBookItem[]? OrderBookItems => OrderBookStateInstrument.Asks;
    }
}
