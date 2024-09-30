using CryptoCurrencyExchangeBrokerLib;
using System.Text.Json;

namespace CryptoCurrencyExchangeBrokerConsole;
public class LocalMarketDataEventListener : IMarketDataEventListener
{
    public bool LogMessageReceived { get; set; }
    public void ExceptionThrown(Exception ex)
    {
        Exception? ex2 = ex;
        while (ex2 != null)
        {
            Console.WriteLine(ex2.Message);
            Console.WriteLine(ex2.StackTrace);
            ex2 = ex2.InnerException;
        }
    }

    public void ExchangeConnected(string url)
    {
        Console.WriteLine($"ExchangeConnected to {url}");
    }

    public void ExchangeDiconnected()
    {
        Console.WriteLine("ExchangeDiconnected");
    }

    public void MessageListenerFinished()
    {
        Console.WriteLine("ExchangeFinished");
    }

    public void MessageListenerStarting()
    {
        Console.WriteLine("MessageListenerStarting");
    }

    public void MessageListenerRestarting()
    {
        Console.WriteLine("MessageListenerRestarting");
    }

    public void MessageReceived(string msg)
    {
        if (LogMessageReceived)
        {
            Console.WriteLine(msg);
        }
    }

    public void SendMessage(string msg)
    {
        Console.WriteLine(msg);
    }

    internal void StartLoggingOrderBookState(MarketDataControl marketDataInstance)
    {
        Task.Factory.StartNew(() =>
        {
            var orderBookState = marketDataInstance.OrderBookState;
            while (marketDataInstance.Status == MarketDataStatusEnum.Started)
            {
                Console.WriteLine($"bid={orderBookState.BidPrice.ToString("f4")} ask={orderBookState.AskPrice.ToString("f4")} mid={orderBookState.MidPrice.ToString("f4")} avg5={orderBookState.AvgMidPrice5Sec.ToString("f4")}");
                Thread.Sleep(5000);
            }
        });
    }
}