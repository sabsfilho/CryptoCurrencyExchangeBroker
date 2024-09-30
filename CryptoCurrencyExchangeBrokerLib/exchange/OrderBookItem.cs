namespace CryptoCurrencyExchangeBrokerLib.exchange
{
    public class OrderBookItem : AExchangeData
    {
        /// <summary>
        /// Cryptocurrency Amount
        /// </summary>
        public required decimal Amount { get; set; }
        /// <summary>
        /// Traditional Monetary Unit Price
        /// </summary>
        public required decimal Price { get; set; }
    }
}
