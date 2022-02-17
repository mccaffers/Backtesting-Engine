using backtesting_engine;
using backtesting_engine_models;

namespace Utilities;

public class DictoinaryKeyStrings
{
    public static string OpenTrade(PriceObj priceObj){
        return priceObj.symbol + "-" + priceObj.date;
    }

    public static string CloseTradeKey(TradeHistoryObject openTradeObj){
        return  ""+openTradeObj.symbol+"-"+openTradeObj.openDate+"-"+openTradeObj.level;
    }
}
