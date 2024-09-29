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
        public required Dictionary<int, decimal> Bids { get; set; }
        public required Dictionary<int, decimal> Asks { get; set; }
    }
}
