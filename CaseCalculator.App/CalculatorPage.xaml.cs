using System.Web;
using System.Net.Http.Json;
using CaseCalculator.Models.Steam;
using CaseCalculator.Models.ExchangeRate;
using CaseCalculator.Models.ExchangeRate.Responses;

namespace CaseCalculator.App;

public partial class Calculator : ContentPage
{
    private static List<Currency> Currency = new List<Currency>
    {
        new Currency
        {
            Description = "United States Dollar",
            Code = "USD"
        },
        new Currency
        {
            Description = "British Pound Sterling",
            Code = "GBP"
        },
        new Currency
        {
            Description = "The Euro",
            Code = "EUR"
        },
        new Currency
        {
            Description = "Swiss Franc",
            Code = "CHF"
        },
        new Currency
        {
            Description = "Russian Ruble",
            Code = "RUB"
        }
    };

    private static Dictionary<string, MarketPrice?> AllCases = new Dictionary<string, MarketPrice?>
    {
        { "Chroma 2 Case", default },
        { "Chroma Case", default },
        { "Chroma 3 Case", default },
        { "Clutch Case", default },
        { "CS20 Case", default },
        { "CS:GO Weapon Case", default },
        { "CS:GO Weapon Case 2", default },
        { "CS:GO Weapon Case 3", default },
        { "Danger Zone Case", default },
        { "Dreams & Nightmares Case", default },
        { "eSports 2013 Case", default },
        { "eSports 2013 Winter Case", default },
        { "eSports 2014 Summer Case", default },
        { "Falchion Case", default },
        { "Fracture Case", default },
        { "Gamma 2 Case", default },
        { "Gamma Case", default },
        { "Glove Case", default },
        { "Horizon Case", default },
        { "Huntsman Weapon Case", default },
        { "Operation Bravo Case", default },
        { "Operation Breakout Weapon Case", default },
        { "Operation Broken Fang Case", default },
        { "Operation Hydra Case", default },
        { "Operation Phoenix Weapon Case", default },
        { "Operation Riptide Case", default },
        { "Operation Vanguard Weapon Case", default },
        { "Operation Wildfire Case", default },
        { "Prisma 2 Case", default },
        { "Prisma Case", default },
        { "Recoil Case", default },
        { "Revolver Case", default },
        { "Shadow Case", default },
        { "Shattered Web Case", default },
        { "Snakebite Case", default },
        { "Spectrum 2 Case", default },
        { "Spectrum Case", default },
        { "Winter Offensive Weapon Case", default },
        { "X-Ray P250 Package", default }
    };

    private double KeyPrice = 2.62;
    private int CaseAmount = 0;

    private Currency? SelectedCurrency;
    private string? MarketRequestItem;
    private MarketPrice? MarketData;

    public Calculator()
    {
        InitializeComponent();
        ResetCalculation();

        MarketPriceLabel.Text = $"${0.00:N2}";
        CurrencyPicker.SelectedIndexChanged += CurrencyPicker_SelectedIndexChanged;
        CasePicker.ItemsSource = AllCases.Select(x => x.Key).OrderBy(x => x).ToList();
    }

    protected override void OnAppearing()
    {
        Task.Run(async () =>
        {
            try
            {
                using var client = new HttpClient();
                var result = await client.GetFromJsonAsync<SupportedSymbols>("https://api.exchangerate.host/symbols");
                if (result?.Symbols is not null)
                {
                    Currency = result.Symbols;
                    await Dispatcher.DispatchAsync(() =>
                    {
                        CurrencyPicker.ItemsSource = Currency;
                        CurrencyPicker.ItemDisplayBinding = new Binding("Code");
                        CurrencyPicker.SelectedIndex = 150;

                        CurrencyActivity.IsVisible = false;
                        CurrencyActivity.IsRunning = false;
                        CurrencyPicker.IsVisible = true;
                        SelectedCurrency = (Currency)CurrencyPicker.SelectedItem;
                    });
                }
            }
            catch (Exception e)
            {
                CurrencyErrorLabel.Text = "Failed to load currency...";
                CurrencyErrorLabel.IsVisible = true;
                CurrencyPicker.IsVisible = false;
                CurrencyActivity.IsVisible = false;
                CurrencyActivity.IsRunning = false;
            }
        });
        base.OnAppearing();
    }

    private void ResetCalculation()
    {
        CaseAmount = 0;
        EstimatedCost.Text = $"${0.00:N2}";
    }

    private async Task GatherMarketData()
    {
        if (CasePicker.SelectedItem is null)
        {
            return;
        }

        MarketRequestItem = CasePicker.SelectedItem.ToString();

        if (MarketRequestItem is not null && AllCases[MarketRequestItem] is not null)
        {
            await UpdateMarketPrice(AllCases[MarketRequestItem]?.MedianPrice ?? 0);
            MarketData = AllCases[MarketRequestItem];
            await RunCalculation();
            return;
        }

        MarketPriceLabel.IsVisible = false;
        MarketPriceActivity.IsRunning = true;

        var hashedName = HttpUtility.UrlEncode(CasePicker.SelectedItem.ToString());
        var requestUrl = $"https://steamcommunity.com/market/priceoverview/?appid=730&currency=1&market_hash_name={hashedName}";

        try
        {
            using var client = new HttpClient();
            MarketData = await client.GetFromJsonAsync<MarketPrice>(requestUrl);

            AllCases[MarketRequestItem!] = MarketData;
        }
        catch (HttpRequestException)
        {
            MarketPriceLabel.Text = $"Failed to contact Steam...";
            MarketPriceActivity.IsRunning = false;
            MarketPriceLabel.IsVisible = true;
            MarketData = null;
            return;
        }

        await UpdateMarketPrice(MarketData?.MedianPrice ?? 0);
        await RunCalculation();
    }

    private async Task RunCalculation()
    {
        if (MarketData is null || CaseAmount == 0)
        {
            ResetCalculation();
            return;
        }

        var casePriceTotal = CaseAmount * MarketData.MedianPrice;
        var caseKeyPriceTotal = KeyPrice * CaseAmount;

        await UpdateEstimatedCost(casePriceTotal + caseKeyPriceTotal);
    }

    private async Task<double> DoCurrencyConversion(double? input)
    {
        if (input is null || SelectedCurrency is null)
        {
            return default;
        }

        if (SelectedCurrency.Code.Equals("USD"))
        {
            return input.Value;
        }

        try
        {
            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<CurrencyConvert>($"https://api.exchangerate.host/convert?from=USD&to={SelectedCurrency?.Code}&amount={input.Value}");
            return response?.Result ?? default;
        }
        catch (Exception e)
        {

            throw;
        }
    }

    private async Task UpdateEstimatedCost(double value)
    {
        EstimatedCost.IsVisible = false;
        EstimatedCostActivity.IsRunning = true;
        var estimatedCost = await DoCurrencyConversion(value);
        EstimatedCost.Text = $"${estimatedCost:N2}";
        EstimatedCostActivity.IsRunning = false;
        EstimatedCost.IsVisible = true;
    }

    private async Task UpdateMarketPrice(double value)
    {
        MarketPriceLabel.IsVisible = false;
        MarketPriceActivity.IsRunning = true;
        var marketPrice = await DoCurrencyConversion(value);
        MarketPriceLabel.Text = $"${marketPrice:N2}";
        MarketPriceActivity.IsRunning = false;
        MarketPriceLabel.IsVisible = true;
    }

    #region Event Handlers
    private async void CurrencyPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var picker = sender as Picker;
        if (picker?.SelectedItem is null || CasePicker.SelectedItem is null)
        {
            return;
        }

        SelectedCurrency = picker?.SelectedItem as Currency;

        await GatherMarketData();
    }

    private async void CasePicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        await GatherMarketData();
    }

    private async void CaseAmountEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(e.NewTextValue, out var amount))
        {
            ResetCalculation();
            return;
        }

        CaseAmount = amount;
        await RunCalculation();
    }
    #endregion
}

