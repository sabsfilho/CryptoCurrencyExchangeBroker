namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketDataEventListener
{
    void ExchangeConnected(string url);
    void ExchangeDiconnected();
    void MessageListenerFinished();
    void MessageListenerStarting();
    void MessageReceived(string msg);
    void SendMessage(string msg);
}