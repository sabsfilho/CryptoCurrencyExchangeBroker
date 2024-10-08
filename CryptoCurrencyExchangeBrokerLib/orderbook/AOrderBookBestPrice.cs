﻿using CryptoCurrencyExchangeBrokerLib.exchange;

namespace CryptoCurrencyExchangeBrokerLib.orderbook
{
    internal abstract class AOrderBookBestPrice
    {
        protected abstract OrderBookItem[]? OrderBookItems { get; }
        protected abstract bool Buy { get; }
        public OrderBookBestPriceState? State { get; private set; }

        protected OrderBookState OrderBookStateInstrument { get; private set; }
        private decimal cryptoAmount;

        public AOrderBookBestPrice(OrderBookState orderBookStateInstrument, decimal cryptoAmount)
        {
            if (cryptoAmount <= 0)
                throw new MarketDataException($"{cryptoAmount} amount is invalid");
            OrderBookStateInstrument = orderBookStateInstrument;
            this.cryptoAmount = cryptoAmount;

            State = CalcValue();
        }

        private OrderBookBestPriceState? CalcValue()
        {
            var orderBookItems = OrderBookItems;

            if (orderBookItems == null || orderBookItems.Length == 0)
                return null;

            decimal amount = cryptoAmount;
            decimal sum = 0;
            foreach (var x in orderBookItems)
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

            return
                new OrderBookBestPriceState()
                {
                    ID = Guid.NewGuid().ToString(),
                    Instrument = OrderBookStateInstrument.Instrument,
                    Buy = Buy,
                    Amount = cryptoAmount,
                    Value = sum / cryptoAmount,
                    OrderBookItems = orderBookItems
                };
        }
    }
}
