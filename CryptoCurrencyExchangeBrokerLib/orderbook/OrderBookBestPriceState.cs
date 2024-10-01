using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    public class OrderBookBestPriceState
    {
        public required string ID { get; set; }
        public required string Instrument { get; set; }
        public required bool Buy { get; set; }
        public required decimal Amount { get; set; }
        public required decimal Value { get; set; }
        public required OrderBookItem[] OrderBookItems { get; set; }
    }
}
