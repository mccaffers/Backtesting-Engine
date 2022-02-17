using backtesting_engine;
using Utilities;

namespace backtesting_engine_models;

public class RequestObject {
    public RequestObject(PriceObj priceObj)
    {
        this.priceObj = priceObj;
        this.openDate = priceObj.date;
        this.key = DictoinaryKeyStrings.OpenTrade(priceObj);
        this.symbol = priceObj.symbol;
    }

    // When setting the direction, set the current level (ASK/BID)
    private TradeDirection _direction;
    public TradeDirection direction {
        get {
            return _direction;
        }
        set {
            _direction = value;
            this.level = _direction == TradeDirection.BUY ? this.priceObj.ask : this.priceObj.bid;
        }
    }

    public decimal stopDistancePips {get; init;}
    public decimal limitDistancePips {get; init;}
    
    // Read only properties, defined in the constructor
    public string key {get;}
    public PriceObj priceObj { get; }
    public DateTime openDate {get;}
    public string? symbol {get;}

    // Private propertises
    public decimal level {get; private set;}

    // Can only be set in the initialisation of the object
    public decimal size {get;init;}

    // Public propertises, for adjustments
    public decimal stopLevel {get; set;}
    public decimal limitLevel {get; set;}

    public void UpdateLevelWithSlippage(decimal slippage){
        this.level = this.direction == TradeDirection.SELL? this.level-slippage : this.level+slippage;
    }
}
