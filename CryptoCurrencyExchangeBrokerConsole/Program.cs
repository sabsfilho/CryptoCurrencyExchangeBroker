﻿using BitstampLib;
using CryptoCurrencyExchangeBrokerConsole;
using CryptoCurrencyExchangeBrokerLib;
using PersistenceLayerCosmosDBLib;

Console.WriteLine("CryptoCurrencyExchangeBroker Bitstamp version");
Console.WriteLine("** This version subscribes btcusd and ethusd instruments. **\n");

const bool LOCAL_TEST_MODE = false;

bool LOG_MESSAGE_RECEIVED = false;
bool LOG_ORDER_BOOK_STATE = false;
bool LOG_GET_BEST_PRICE = false;
bool LOG_GET_BEST_PRICE_FULL_DETAIL = false;

if (!LOCAL_TEST_MODE)
{
    LOG_MESSAGE_RECEIVED = BuildYesMessage("Do you want to log all messages received from Websocket");
    LOG_ORDER_BOOK_STATE = BuildYesMessage("Do you want to log Order Book values on each 5 sec");
    LOG_GET_BEST_PRICE = BuildYesMessage("Do you want to log current best prices");
    LOG_GET_BEST_PRICE_FULL_DETAIL = BuildYesMessage("Do you want to log current best prices including full detail information");
}


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

var databaseWriter = new CosmosDBWriter(localListener)
{
    WriteLimitPerSession = 10
};

foreach (var intrument in instruments)
{
    var marketDataInstance =
        MarketDataControl.SubscribeOrderBook(
            intrument,
            provider,
            localListener,
            databaseWriter
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


static bool BuildYesMessage(string m)
{
    Console.WriteLine($"\n{m} ? [Y]");
    return new List<char> { 'Y', 'y' }.Contains(Console.ReadKey().KeyChar);
}