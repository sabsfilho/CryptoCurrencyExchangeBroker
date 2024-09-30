using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    public class OrderBookState
    {
        private object locker = new object();
        private Dictionary<string, OrderBookStateInstrument> OrderBookStateInstruments { get; set; }

        public OrderBookState()
        {
            OrderBookStateInstruments = new Dictionary<string, OrderBookStateInstrument>();
        }

        public OrderBookStateInstrument? GetState(string instrument)
        {
            if (!OrderBookStateInstruments.ContainsKey(instrument))
                return null;

            return OrderBookStateInstruments[instrument];
        }
        public OrderBookStateInstrument[] GetStates()
        {
            return OrderBookStateInstruments.Values.ToArray();
        }

        internal void MessageReceived(AExchangeData? exchangeData)
        {
            lock (locker)
            {
                var orderBook = exchangeData as OrderBook;
                if (orderBook == null)
                    return;

                string instrument = orderBook.Instrument;
                OrderBookStateInstrument o;
                if (!OrderBookStateInstruments.TryGetValue(instrument, out o!))
                {
                    o = new OrderBookStateInstrument(instrument);
                    OrderBookStateInstruments.Add(instrument, o);
                }

                o.MessageReceived(orderBook);
            }
        }
    }
}
