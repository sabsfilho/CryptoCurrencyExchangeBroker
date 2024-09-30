using BitstampLib;
using CryptoCurrencyExchangeBrokerConsole;
using CryptoCurrencyExchangeBrokerLib;

const bool LOG_MESSAGE_RECEIVED = false;
const bool LOG_ORDER_BOOK_STATE = false;
const bool LOG_GET_BEST_PRICE = false;
const bool LOG_GET_BEST_PRICE_FULL_DETAIL = true;

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
        LogMessageReceived = LOG_MESSAGE_RECEIVED
    };

foreach (var intrument in instruments)
{
    var marketDataInstance =
        MarketDataControl.SubscribeOrderBook(
            intrument,
            provider,
            localListener
        );

    if (LOG_ORDER_BOOK_STATE)
        localListener.StartLoggingOrderBookState(marketDataInstance);

    if (LOG_GET_BEST_PRICE)
        localListener.StartLoggingGetBestPrice(marketDataInstance);

    if (LOG_GET_BEST_PRICE_FULL_DETAIL)
        localListener.StartLoggingGetBestPriceFullDetail(marketDataInstance);

    marketDataInstances.Add(marketDataInstance);
}

Console.ReadKey();

foreach (var marketDataInstance in marketDataInstances)
{
    marketDataInstance.Stop();
}

Console.WriteLine("exit");