namespace CryptoCurrencyExchangeBrokerLib.exchange
{
    public class OrderBook : AExchangeData
    {
        public required DateTime Timestamp { get; set; }
        public required string Instrument { get; set; }
        public required OrderBookItem[] Bids { get; set; }
        public required OrderBookItem[] Asks { get; set; }
    }
}
