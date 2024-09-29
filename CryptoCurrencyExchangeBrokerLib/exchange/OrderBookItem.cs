namespace CryptoCurrencyExchangeBrokerLib.exchange
{
    public class OrderBookItem : AExchangeData
    {
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
    }
}
