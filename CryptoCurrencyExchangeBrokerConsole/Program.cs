using BitstampLib;
using CryptoCurrencyExchangeBrokerConsole;
using CryptoCurrencyExchangeBrokerLib;

Console.WriteLine("CryptoCurrencyExchangeBroker running...");
Console.WriteLine("\n\n*** Press any key to stop. ***\n\n");

var localListener =
    new LocalMarketDataEventListener()
    {
        LogMessageReceived = false
    };

var marketDataInstance = new MarketDataControl(
    new BitstampProvider(),
    localListener
);

marketDataInstance.Subscribe(
    CryptoCurrencyExchangeBrokerLib.ChannelEnum.OrderBook, 
    "btcusd"
);

localListener.StartLoggingOrderBookState(marketDataInstance);

Console.ReadKey();

marketDataInstance.Stop();

Console.WriteLine("exit");