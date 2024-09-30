using BitstampLib;
using CryptoCurrencyExchangeBrokerConsole;
using CryptoCurrencyExchangeBrokerLib;

Console.WriteLine("CryptoCurrencyExchangeBroker running...");
Console.WriteLine("\n\n*** Press any key to stop. ***\n\n");

var instruments = new string[]
{
    "btcusd",
    "ethusd"
};
var marketDataInstances = new List<MarketDataControl>();
var provider = new BitstampProvider();
var localListener =
    new LocalMarketDataEventListener()
    {
        LogMessageReceived = false
    };

foreach (var intrument in instruments)
{
    var marketDataInstance = 
        MarketDataControl.SubscribeOrderBook(
            provider,
            localListener,
            intrument
        );

    localListener.StartLoggingOrderBookState(marketDataInstance);

    localListener.StartLoggingGetBestPrice(marketDataInstance);

}


Console.ReadKey();

foreach (var marketDataInstance in marketDataInstances)
{
    marketDataInstance.Stop();
}

Console.WriteLine("exit");