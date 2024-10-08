﻿using CryptoCurrencyExchangeBrokerLib.orderbook;

namespace CryptoCurrencyExchangeBrokerLib;
public interface IMarketData
{
    string Instrument { get; }
    MarketDataStatusEnum Status { get; }
    void Start();
    void Stop();
    void Subscribe(ChannelEnum channel);
    void Unsubscribe(ChannelEnum channel);
    OrderBookBestPriceState? GetBestPrice(bool buy, decimal cryptoAmount);
}