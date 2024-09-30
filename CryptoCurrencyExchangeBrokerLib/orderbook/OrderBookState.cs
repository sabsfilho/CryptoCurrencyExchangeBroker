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
        class TimeItem
        {
            public required DateTime Timestamp { get; set; }
            public required decimal Value { get; set; }
        }

        private object locker = new object();

        private Stack<TimeItem> midPrices;
        private DateTime lastClearMidPriceTime;

        public string Instrument { get; private set; }

        public OrderBookState(string instrument)
        {
            midPrices = new Stack<TimeItem>();
            Instrument = instrument;
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
        public OrderBookItem? Ask
        {
            get
            {
                lock (locker)
                {
                    return
                        asks != null && asks.Length > 0 ?
                        asks[0] :
                        null;
                }
            }
        }
        public OrderBookItem? Bid
        {
            get
            {
                lock (locker)
                {
                    return
                        bids != null && bids.Length > 0 ?
                        bids[0] :
                        null;
                }
            }
        }

        public decimal AskPrice
        {
            get
            {
                lock (locker)
                {
                    return
                        Ask == null ? 0 : Ask.Price;
                }
            }
        }

        public decimal BidPrice
        {
            get
            {
                lock (locker)
                {
                    return
                        Bid == null ? 0 : Bid.Price;
                }
            }
        }
        public decimal MidPrice
        {
            get
            {
                lock (locker)
                {
                    return (AskPrice + BidPrice) / 2m;
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

        internal void MessageReceived(AExchangeData? exchangeData)
        {
            lock (locker)
            {
                var orderBook = exchangeData as OrderBook;
                if (orderBook == null)
                    return;

                asks = orderBook.Asks;
                bids = orderBook.Bids;

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
