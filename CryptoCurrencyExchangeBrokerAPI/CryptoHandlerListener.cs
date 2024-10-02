using CryptoCurrencyExchangeBrokerLib;
using PersistenceLayerCosmosDBLib;
using System;

namespace CryptoCurrencyExchangeBrokerAPI
{
    public class CryptoHandlerListener : IMarketDataEventListener, IDatabaseListener
    {
        private ILogger logger;

        #region IMarketDataEventListener

        public bool LogMessageReceived { get; set; }

        public CryptoHandlerListener(ILogger logger)
        {
            this.logger = logger;
        }

        public void ExceptionThrown(Exception ex)
        {
            logger.LogError(ex, "ExceptionThrown");
        }

        public void ExchangeConnected(string url)
        {
            logger.LogInformation($"ExchangeConnected to {url}");
        }

        public void ExchangeDiconnected()
        {
            logger.LogInformation("ExchangeDiconnected");
        }

        public void MessageListenerFinished()
        {
            logger.LogInformation("ExchangeFinished");
        }

        public void MessageListenerRunning()
        {
            logger.LogInformation("MessageListenerRunning");
        }

        public void MessageListenerRestarting()
        {
            logger.LogInformation("MessageListenerRestarting");
        }

        public void MessageReceived(string msg)
        {
            if (LogMessageReceived)
            {
                logger.LogInformation(msg);
            }
        }

        public void SendMessage(string msg)
        {
            logger.LogInformation(msg);
        }
        #endregion
        #region IDatabaseListener
        public void SaveChangesFailed(Exception ex)
        {
            ExceptionThrown(ex);
        }
        #endregion

    }
}
