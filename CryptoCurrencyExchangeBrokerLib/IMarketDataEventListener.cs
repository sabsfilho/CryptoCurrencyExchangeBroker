
namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketDataEventListener
{
    void ExceptionThrown(Exception ex);
    void ExchangeConnected(string url);
    void ExchangeDiconnected();
    void MessageListenerFinished();
    void MessageListenerStarting();
    void MessageListenerRestarting();
    void MessageReceived(string msg);
    void SendMessage(string msg);
}