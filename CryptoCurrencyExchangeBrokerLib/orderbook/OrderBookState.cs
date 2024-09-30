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

        public OrderBookState()
        {
            midPrices = new Stack<TimeItem>();
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

        internal void MessageReceived(AExchangeData? exchangeData)
        {
            lock (locker)
            {
                var orderBook = exchangeData as OrderBook;
                if (orderBook == null)
                    return;

                askPrice = orderBook.Asks[0].Price;
                bidPrice = orderBook.Bids[0].Price;

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
