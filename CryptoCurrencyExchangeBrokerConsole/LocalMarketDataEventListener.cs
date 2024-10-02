using CryptoCurrencyExchangeBrokerLib;
using PersistenceLayerCosmosDBLib;
using System.Text.Json;

namespace CryptoCurrencyExchangeBrokerConsole;
public class LocalMarketDataEventListener : IMarketDataEventListener, IDatabaseListener
{
    #region IMarketDataEventListener
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

    public void MessageListenerRunning()
    {
        Console.WriteLine("MessageListenerRunning");
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
        Task.Factory.StartNew((Action)(() =>
        {
            var orderBookState = marketDataInstance.OrderBookState;
            while (marketDataInstance.Status == MarketDataStatusEnum.Started)
            {
                var bid = orderBookState.Bid;
                var ask = orderBookState.Ask;
                if (bid != null && ask != null)
                {
                    string bidtxt = $"bid={orderBookState.Bid!.Amount.ToString("f4")}@{orderBookState.Bid.Price.ToString("f4")}";
                    string asktxt = $"ask={orderBookState.Ask!.Amount.ToString("f4")}@{orderBookState.Ask.Price.ToString("f4")}";
                    Console.WriteLine($"read {orderBookState.Instrument} {bidtxt} {asktxt} mid={orderBookState.MidPrice.ToString("f4")} avg5={orderBookState.AvgMidPrice5Sec.ToString("f4")}");
                }
                Thread.Sleep(5000);
            }
        }));
    }

    internal void StartLoggingGetBestPrice(MarketDataControl marketDataInstance)
    {
        Task.Factory.StartNew(() =>
        {
            var orderBookState = marketDataInstance.OrderBookState;
            while (marketDataInstance.Status == MarketDataStatusEnum.Started)
            {
                var random = new Random();
                if (marketDataInstance.Subscribed)
                {
                    bool buy = random.NextDouble() < 0.5;
                    decimal amount = Convert.ToDecimal(random.NextDouble());
                    var state = marketDataInstance.GetBestPrice(buy, amount);
                    if (state == null)
                        continue;

                    Console.WriteLine($"{(buy ? "buy" : "sell")} {marketDataInstance.Instrument} {amount} = {state.Value}");
                }
                Thread.Sleep(random.Next(3, 7) * 1000);
            }
        });
    }

    internal void StartLoggingGetBestPriceFullDetail(MarketDataControl marketDataInstance)
    {
        Task.Factory.StartNew(() =>
        {
            var orderBookState = marketDataInstance.OrderBookState;
            while (marketDataInstance.Status == MarketDataStatusEnum.Started)
            {
                var random = new Random();
                if (marketDataInstance.Subscribed)
                {
                    bool buy = random.NextDouble() < 0.5;
                    decimal amount = Convert.ToDecimal(random.NextDouble());
                    var state = marketDataInstance.GetBestPrice(buy, amount);
                    if (state == null)
                        continue;

                    string json = JsonSerializer.Serialize(state);

                    Console.WriteLine(json);
                }
                Thread.Sleep(60_000);
            }
        });
    }
    #endregion
    #region IDatabaseListener
    public void SaveChangesFailed(Exception ex)
    {
        ExceptionThrown(ex);
    }
    #endregion
}