using CryptoCurrencyExchangeBrokerLib;

namespace CryptoCurrencyExchangeBrokerConsole;
public class LocalMarketDataEventListener : IMarketDataEventListener
{
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
        Console.WriteLine(msg);
    }

    public void SendMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}