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
                var xs = orderBookState.GetStates();
                foreach (var x in xs)
                {
                    Console.WriteLine($"read {x.Instrument} bid={x.BidPrice.ToString("f4")} ask={x.AskPrice.ToString("f4")} mid={x.MidPrice.ToString("f4")} avg5={x.AvgMidPrice5Sec.ToString("f4")}");
                }
                Thread.Sleep(5000);
            }
        });
    }
    internal void StartLoggingGetBestPrice(MarketDataControl marketDataInstance)
    {
        Task.Factory.StartNew(() =>
        {
            var orderBookState = marketDataInstance.OrderBookState;
            while (marketDataInstance.Status == MarketDataStatusEnum.Started)
            {
                if (marketDataInstance.Subscribed)
                {
                    int k = Random.Shared.Next(1, 10);

                    bool buy = k < 5;
                    string instrument = k < 5 ? "btcusd" : "ethusd";
                    decimal amount = Convert.ToDecimal(Random.Shared.NextDouble());
                    decimal v = marketDataInstance.GetBestPrice(false, instrument, 1m);

                    Console.WriteLine($"{(buy ? "buy" : "sell")} {instrument} {amount} = {v}");
                }
                Thread.Sleep(Random.Shared.Next(3, 7) * 1000);
            }
        });
    }
}