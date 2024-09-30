namespace CryptoCurrencyExchangeBrokerLib.exchange
{
    public class OrderBookItem : AExchangeData
    {
        /// <summary>
        /// Cryptocurrency Amount
        /// </summary>
        public decimal Amount { get; private set; }
        /// <summary>
        /// Traditional Monetary Unit Price
        /// </summary>
        public decimal Price { get; private set; }
        public OrderBookItem(decimal amount, decimal price)
        {
            Amount = amount;
            Price = price;
        }
    }
}
