using CryptoCurrencyExchangeBrokerLib;

namespace CryptoCurrencyExchangeBrokerConsole;
public class LocalMarketDataEventListener : IMarketDataEventListener
{
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

    public void MessageReceived(string msg)
    {
        Console.WriteLine(msg);
    }

    public void SendMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}