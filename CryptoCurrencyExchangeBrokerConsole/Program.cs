using BitstampLib;
using CryptoCurrencyExchangeBrokerConsole;
using CryptoCurrencyExchangeBrokerLib;

Console.WriteLine("CryptoCurrencyExchangeBroker running...");
              
var marketDataInstance = new MarketDataControl(
    new BitstampProvider(),
    new LocalMarketDataEventListener()
);
marketDataInstance.Subscribe(
    CryptoCurrencyExchangeBrokerLib.ChannelEnum.OrderBook, 
    "btcusd"
);

Console.ReadKey();

marketDataInstance.Stop();

Console.WriteLine("exit");