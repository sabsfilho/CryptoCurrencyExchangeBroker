using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    public class OrderBookStateList
    {
        private object locker = new object();
        private Dictionary<string, OrderBookState> OrderBookStateInstruments { get; set; }

        public OrderBookStateList()
        {
            OrderBookStateInstruments = new Dictionary<string, OrderBookState>();
        }

        public OrderBookState? GetState(string instrument)
        {
            if (!OrderBookStateInstruments.ContainsKey(instrument))
                return null;

            return OrderBookStateInstruments[instrument];
        }
        public OrderBookState[] GetStates()
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
                OrderBookState o;
                if (!OrderBookStateInstruments.TryGetValue(instrument, out o!))
                {
                    o = new OrderBookState(instrument);
                    OrderBookStateInstruments.Add(instrument, o);
                }

                o.MessageReceived(orderBook);
            }
        }
    }
}
