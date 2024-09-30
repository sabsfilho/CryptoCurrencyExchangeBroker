using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    public class OrderBookStateInstrument
    {
        class TimeItem
        {
            public required DateTime Timestamp { get; set; }
            public required decimal Value { get; set; }
        }

        private object locker = new object();

        private Stack<TimeItem> midPrices;
        private DateTime lastClearMidPriceTime;

        public string Instrument { get; private set; }

        public OrderBookStateInstrument(string instrument)
        {
            midPrices = new Stack<TimeItem>();
            Instrument = instrument;
        }

        private decimal askPrice;
        public decimal AskPrice
        {
            get
            {
                lock (locker)
                {
                    return askPrice;
                }
            }
        }
        private decimal bidPrice;

        public decimal BidPrice
        {
            get
            {
                lock (locker)
                {
                    return bidPrice;
                }
            }
        }
        public decimal MidPrice
        {
            get
            {
                lock (locker)
                {
                    return (askPrice + bidPrice) / 2m;
                }
            }
        }
        public decimal AvgMidPrice5Sec
        {
            get
            {
                lock (locker)
                {
                    ClearMidPrice(DateTime.Now);

                    return
                        midPrices.Count == 0 ? 0 :
                        midPrices.Average(x => x.Value);
                }
            }
        }

        private OrderBookItem[]? asks;
        public OrderBookItem[]? Asks
        {
            get
            {
                lock (locker)
                {
                    return asks;
                }
            }
        }

        private OrderBookItem[]? bids;
        public OrderBookItem[]? Bids
        {
            get
            {
                lock (locker)
                {
                    return bids;
                }
            }
        }

        internal void MessageReceived(OrderBook orderBook)
        {
            lock (locker)
            {
                asks = orderBook.Asks;
                bids = orderBook.Bids;

                askPrice = asks[0].Price;
                bidPrice = bids[0].Price;

                AddMidPrice();

            }
        }

        private void AddMidPrice()
        {
            ClearMidPrice(DateTime.Now);
            midPrices.Push(new TimeItem()
            {
                Timestamp = DateTime.Now,
                Value = MidPrice
            });
        }
        private void ClearMidPrice(DateTime now)
        {
            if ((now - lastClearMidPriceTime).TotalSeconds < 1)
                return;

            lastClearMidPriceTime = now;

            var tm = new DateTime();
            while (midPrices.Count > 0)
            {
                var tmi = midPrices.Peek().Timestamp;
                if (tmi == tm)
                    break;
                else if (tm == new DateTime())
                {
                    tm = tmi;
                }
                if ((now - tmi).TotalSeconds > 5)
                {
                    midPrices.Pop();
                }

            }
        }
    }
}
