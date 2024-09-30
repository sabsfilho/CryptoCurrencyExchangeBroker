using CryptoCurrencyExchangeBrokerLib.exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal abstract class AOrderBookBestPrice
    {
        protected abstract OrderBookItem[]? OrderBookItems { get; }
        public decimal Value { get; private set; }

        protected OrderBookStateInstrument OrderBookStateInstrument { get; private set; }
        private decimal cryptoAmount;

        public AOrderBookBestPrice(OrderBookStateInstrument orderBookStateInstrument, decimal cryptoAmount)
        {
            if (cryptoAmount <= 0)
                throw new MarketDataException($"{cryptoAmount} amount is invalid");
            OrderBookStateInstrument = orderBookStateInstrument;
            this.cryptoAmount = cryptoAmount;

            Value = CalcValue();
        }

        private decimal CalcValue()
        {
            var xs = OrderBookItems;
            
            if (xs == null || xs.Length == 0)
                return 0;

            decimal amount = cryptoAmount;
            decimal sum = 0;
            foreach(var x in xs)
            {
                decimal a = x.Amount;
                decimal p = x.Price;

                if (amount <= a)
                {
                    sum += amount * p;
                    break;
                }
                sum += a * p;
                amount -= a;
            }
            return sum / cryptoAmount;
        }
    }
}
