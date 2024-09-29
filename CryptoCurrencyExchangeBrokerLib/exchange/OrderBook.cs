using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.exchange
{
    public class OrderBook : AExchangeData
    {
        public required DateTime Timestamp { get; set; }
        public required string Ticker { get; set; }
        public required OrderBookItem[] Bids { get; set; }
        public required OrderBookItem[] Asks { get; set; }
    }
}
