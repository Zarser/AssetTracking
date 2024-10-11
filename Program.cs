using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AssetTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Asset> assets = InitializeAssets();
            Console.WriteLine("Välkommen till Asset Tracking System\n");

            while (true)
            {
                // Get country and currency
                var (selectedCountry, selectedCurrency) = GetCountryAndCurrency();
                if (selectedCountry == "Ogiltig") continue;

                // Get asset type
                var assetType = GetAssetType();
                if (assetType == null) continue;

                // Filter and display assets
                var selectedAssets = assets
                    .Where(a => a.Country == selectedCountry && a.AssetType == assetType)
                    .OrderByDescending(a => a.PurchaseDate)
                    .ToList();

                var conversionRates = GetCurrencyConversionRates(selectedCurrency);
                DisplayAssets(selectedAssets, conversionRates, selectedCurrency);

                // Ask if user wants to search again
                if (!AskToSearchAgain()) break;
            }
        }

        private static (string, string) GetCountryAndCurrency()
        {
            Console.WriteLine("Välj Land/Kontor:\n1. USA (USD)\n2. Tyskland (EUR)\n3. Storbritannien (GBP)\n4. Sverige (SEK)");
            int choice = int.Parse(Console.ReadLine() ?? "0");
            return choice switch
            {
                1 => ("USA", "USD"),
                2 => ("Tyskland", "EUR"),
                3 => ("Storbritannien", "GBP"),
                4 => ("Sverige", "SEK"),
                _ => ("Ogiltig", "USD")
            };
        }

        private static string GetAssetType()
        {
            Console.WriteLine("Välj Tillgångstyp:\n1. Laptops/Datorer\n2. Mobiltelefoner");
            int choice = int.Parse(Console.ReadLine() ?? "0");
            return choice switch
            {
                1 => "Laptop",
                2 => "Mobiltelefon",
                _ => null
            };
        }

        private static Dictionary<string, decimal> GetCurrencyConversionRates(string baseCurrency)
        {
            var rates = new Dictionary<string, decimal> { { "USD", 1m }, { "EUR", 0.85m }, { "GBP", 0.75m }, { "SEK", 8.5m } };
            return rates.ToDictionary(rate => rate.Key, rate => rate.Value / rates[baseCurrency]);
        }

        private static void DisplayAssets(List<Asset> assets, Dictionary<string, decimal> conversionRates, string currency)
        {
            Console.WriteLine($"\nTillgång       Märke      Modell      Pris i {currency}      Datum");
            Console.WriteLine("------------------------------------------------------");
            foreach (var asset in assets)
            {
                decimal convertedPrice = asset.Price * (conversionRates.ContainsKey(currency) ? conversionRates[currency] : 1);
                Console.ForegroundColor = GetConsoleColor(GetColorCode(asset));
                Console.WriteLine($"{asset.AssetType,-12} {asset.Brand,-10} {asset.Model,-10} {convertedPrice.ToString("C", CultureInfo.CurrentCulture),-15} {asset.PurchaseDate.ToShortDateString()}");
            }
            Console.ResetColor();
        }

        private static string GetColorCode(Asset asset)
        {
            int monthsToThreeYears = ((asset.PurchaseDate.AddYears(3).Year - DateTime.Now.Year) * 12) + asset.PurchaseDate.AddYears(3).Month - DateTime.Now.Month;
            return monthsToThreeYears switch
            {
                < 0 => "DEFAULT",
                <= 3 => "RED",
                <= 6 => "YELLOW",
                > 24 => "GREEN",
                _ => "DEFAULT"
            };
        }

        private static ConsoleColor GetConsoleColor(string colorCode) =>
            colorCode switch
            {
                "RED" => ConsoleColor.Red,
                "YELLOW" => ConsoleColor.Yellow,
                "GREEN" => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

        private static bool AskToSearchAgain()
        {
            Console.WriteLine("\nVill du söka igen? (Y/N)");
            return char.ToUpper(Console.ReadKey().KeyChar) == 'Y';
        }

        private static List<Asset> InitializeAssets() => new List<Asset>
        {
            new Asset("Laptop", "Dell", "XPS 13", 1200, new DateTime(2019, 10, 15), "USA"),
            new Asset("Mobiltelefon", "Samsung", "Galaxy S10", 800, new DateTime(2019, 8, 1), "Tyskland"),
            new Asset("Laptop", "HP", "Spectre x360", 1300, new DateTime(2021, 6, 1), "USA"),
            new Asset("Mobiltelefon", "Apple", "iPhone 12", 999, new DateTime(2021, 11, 15), "Tyskland"),
            new Asset("Laptop", "Apple", "MacBook Pro", 2400, new DateTime(2022, 5, 10), "Tyskland"),
            new Asset("Mobiltelefon", "Google", "Pixel 5", 699, new DateTime(2022, 3, 15), "Storbritannien"),
            new Asset("Laptop", "Lenovo", "ThinkPad X1", 1500, new DateTime(2023, 1, 20), "Tyskland"),
            new Asset("Mobiltelefon", "OnePlus", "OnePlus 9", 799, new DateTime(2023, 4, 25), "Storbritannien"),
            new Asset("Laptop", "Asus", "ZenBook", 1400, new DateTime(2022, 7, 20), "USA"),
            new Asset("Mobiltelefon", "Xiaomi", "Mi 11", 749, new DateTime(2022, 10, 1), "Tyskland"),
            new Asset("Mobiltelefon", "Sony", "Xperia 5", 899, new DateTime(2023, 2, 5), "Storbritannien"),
            new Asset("Laptop", "Razer", "Blade 15", 2500, new DateTime(2023, 3, 12), "USA"),
            new Asset("Mobiltelefon", "Nokia", "G50", 299, new DateTime(2021, 5, 1), "Storbritannien"),
            new Asset("Laptop", "Acer", "Aspire 5", 600, new DateTime(2023, 5, 10), "Sverige"),
            new Asset("Mobiltelefon", "Huawei", "P30", 599, new DateTime(2021, 4, 20), "Sverige"),
        };
    }

    class Asset
    {
        public string AssetType { get; }
        public string Brand { get; }
        public string Model { get; }
        public decimal Price { get; }
        public DateTime PurchaseDate { get; }
        public string Country { get; }

        public Asset(string assetType, string brand, string model, decimal price, DateTime purchaseDate, string country)
        {
            AssetType = assetType;
            Brand = brand;
            Model = model;
            Price = price;
            PurchaseDate = purchaseDate;
            Country = country;
        }
    }
}
