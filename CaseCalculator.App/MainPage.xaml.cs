using CaseCalculator.Utilities;
using CaseCalculator.Models;
using System.Web;
using System.Net.Http.Json;
using CaseCalculator.Models.Steam;
using CaseCalculator.Models.ExchangeRate;

namespace CaseCalculator.App;

public partial class MainPage : ContentPage
{

    private static List<Currency> Currencies = new List<Currency>
    {
        new Currency
        {
            Id = 1,
            Name = "United States Dollar",
            ShortName = "USD"
        },
        new Currency
        {
            Id = 2,
            Name = "British Pound Sterling",
            ShortName = "GBP"
        },
        new Currency
        {
            Id = 3,
            Name = "The Euro",
            ShortName = "EUR"
        },
        new Currency
        {
            Id = 4,
            Name = "Swiss Franc",
            ShortName = "CHF"
        },
        new Currency
        {
            Id = 5,
            Name = "Russian Ruble",
            ShortName = "RUB"
        }
    };

    private static List<string> AllCases = new List<string>
    {
        "Chroma 2 Case",
        "Chroma Case",
        "Chroma 3 Case",
        "Clutch Case",
        "CS20 Case",
        "CS:GO Weapon Case",
        "CS:GO Weapon Case 2",
        "CS:GO Weapon Case 3",
        "Danger Zone Case",
        "Dreams & Nightmares Case",
        "eSports 2013 Case",
        "eSports 2013 Winter Case",
        "eSports 2014 Summer Case",
        "Falchion Case",
        "Fracture Case",
        "Gamma 2 Case",
        "Gamma Case",
        "Glove Case",
        "Horizon Case",
        "Huntsman Weapon Case",
        "Operation Bravo Case",
        "Operation Breakout Weapon Case",
        "Operation Broken Fang Case",
        "Operation Hydra Case",
        "Operation Phoenix Case",
        "Operation Riptide Case",
        "Operation Vanguard Weapon Case",
        "Operation Wildfire Case",
        "Prisma 2 Case",
        "Prisma Case",
        "Recoil Case",
        "Revolver Case",
        "Shadow Case",
        "Shattered Web Case",
        "Snakebite Case",
        "Spectrum 2 Case",
        "Spectrum Case",
        "Winter Offensive Weapon Case",
        "X-Ray P250 Package"
    };

    private double KeyPrice = 2.49;
    private double? KeyPriceConversion;

    private string? MarketRequestItem;
    private MarketPrice? MarketData;
    private DateTime? MarketRequestTime;

    public MainPage()
    {
        InitializeComponent();
        ResetCalculation();
        CurrencyPicker.ItemsSource = Currencies;
        CurrencyPicker.ItemDisplayBinding = new Binding("ShortName");
        CurrencyPicker.SelectedIndex = 0;
        CurrencyPicker.SelectedIndexChanged += CurrencyPicker_SelectedIndexChanged;

        CasePicker.ItemsSource = AllCases.OrderBy(x => x).ToList();
        
    }

    private void ResetCalculation()
    {
        MarketPriceLabel.Text = $"${0.00:N2}";
        EstimatedCost.Text = $"${0.00:N2}";
    }

    private async Task GatherMarketData()
    {
        if (CasePicker.SelectedItem is null)
        {
            return;
        }

        int currencyId = 1;
        if (CurrencyPicker.SelectedItem is not null)
        {
            var currency = ((Currency)CurrencyPicker.SelectedItem);
            currencyId = currency.Id;

            if (currencyId != 1)
            {
                var currencyUrl = $"https://api.exchangerate.host/convert?from=USD&to={currency.ShortName}&amount={KeyPrice}";
                using var currencyClient = new HttpClient();

                var currencyResponse = await currencyClient.GetAsync(currencyUrl);
                var currencyResult = await currencyResponse.Content.ReadFromJsonAsync<CurrencyExchange>();
                KeyPriceConversion = currencyResult?.Result;
            } else
            {
                KeyPriceConversion = null;
            }
        }

        MarketRequestItem = CasePicker.SelectedItem.ToString();

        var hashedName = HttpUtility.UrlEncode(MarketRequestItem);
        var requestUrl = $"https://steamcommunity.com/market/priceoverview/?appid=730&currency={currencyId}&market_hash_name={hashedName}";

        using var client = new HttpClient();

        var result = await client.GetAsync(requestUrl);
        MarketData = await result.Content.ReadFromJsonAsync<MarketPrice>();
        MarketRequestTime = DateTime.Now;

        MarketPriceLabel.Text = $"${MarketData?.MedianPrice}";
        RunCalculation();
    }

    private void RunCalculation()
    {
        if (MarketData is null)
        {
            return;
        }

        var casePriceTotal = CaseAmountStepper.Value * MarketData.MedianPrice;

        var caseKeyPriceTotal = KeyPriceConversion is null 
            ? KeyPrice * CaseAmountStepper.Value
            : KeyPriceConversion.Value * CaseAmountStepper.Value;

        EstimatedCost.Text = $"${casePriceTotal + caseKeyPriceTotal:N2}";
    }

    #region Event Handlers
    private async void CurrencyPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        await GatherMarketData();
    }

    private async void CasePicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        await GatherMarketData();
    }

    private void CaseAmountStepper_ValueChanged(object? sender, ValueChangedEventArgs e)
        => RunCalculation();
    #endregion
}

