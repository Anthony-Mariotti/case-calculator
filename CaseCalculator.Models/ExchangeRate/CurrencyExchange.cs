namespace CaseCalculator.Models.ExchangeRate;

public class CurrencyExchange
{
    //public ExchangeQuery Query { get; set; }

    //public ExchangeInfo Info { get; set; }

    //public bool Historical { get; set; }

    public double Result { get; set; }
}

public class ExchangeInfo
{
    public double Rate { get; set; }
}

public class ExchangeQuery
{
    public string From { get; set; }

    public string To { get; set; }

    public double Amount { get; set; }
}
