using System;
using System.Collections.Generic;
using System.Linq;

class Asset
{
    public string ModelName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal Price { get; set; }
    public string AssetType { get; set; }
    public string Brand { get; set; }
    public DateTime EndOfLifeDate => PurchaseDate.AddYears(3); // Slutdatum (livslängd) är 3 år efter inköpsdatum

    // Konstruktor för att skapa ett nytt Asset-objekt
    public Asset(string brand, string modelName, DateTime purchaseDate, decimal price, string assetType)
    {
        Brand = brand;
        ModelName = modelName;
        PurchaseDate = purchaseDate;
        Price = price;
        AssetType = assetType;
    }

    // Kolla om objektet är mindre än 3 månader från att utgå
    public bool IsAboutToExpire() => (EndOfLifeDate - DateTime.Now).TotalDays <= 90 && (EndOfLifeDate - DateTime.Now).TotalDays >= 0;

    // Kolla om objektet redan har utgått
    public bool IsExpired() => DateTime.Now > EndOfLifeDate;

    // Metod för att visa information om tillgången med färgkod
    public void DisplayInfo()
    {
        // Färgkod beroende på om tillgången har utgått eller närmar sig slutdatum
        if (IsExpired())
        {
            Console.ForegroundColor = ConsoleColor.Red; // Markera utgångna tillgångar i rött
        }
        else if (IsAboutToExpire())
        {
            Console.ForegroundColor = ConsoleColor.Yellow; // Markera tillgångar som snart utgår i gult
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green; // Markera tillgångar som är yngre än 3 år i grönt
        }

        // Visa tillgångsinformation i en tabellformat
        Console.WriteLine($"{AssetType,-10} {Brand,-15} {ModelName,-20} {Price,10} SEK {PurchaseDate.ToShortDateString()}");
        Console.ResetColor(); // Återställ till standardfärg
    }
}

class AssetTracker
{
    private List<Asset> assets = new List<Asset>(); // Lista för att lagra tillgångar

    // Lägg till en ny tillgång till listan
    public void AddAsset(string brand, string modelName, DateTime purchaseDate, decimal price, string assetType)
    {
        assets.Add(new Asset(brand, modelName, purchaseDate, price, assetType));
    }

    // Metod för att visa sorterade tillgångar
    public void DisplayAssetsSorted()
    {
        var sortedAssets = assets
            .OrderBy(asset => asset.AssetType) // Sortera efter typ (datorer först, telefoner sen)
            .ThenBy(asset => asset.PurchaseDate); // Sortera efter inköpsdatum

        // Rubriker för tabellen
        Console.WriteLine($"{"Typ",-10} {"Märke",-15} {"Modell",-20} {"Pris",-10} {"Inköpsdatum"}");
        Console.WriteLine(new string('-', 75));

        // Visa varje tillgångs information
        foreach (var asset in sortedAssets)
        {
            asset.DisplayInfo();
        }

        // Visa färgförklaringen under listan
        DisplayColorExplanation();
    }

    // Förklaring av färgerna
    private void DisplayColorExplanation()
    {
        Console.WriteLine();
        Console.WriteLine("Färgförklaring:");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("GRÖN   : Produkten är yngre än 3 år.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("GUL    : Produkten är äldre än 3 år men har mindre än 3 månader kvar till utgång.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("RÖD    : Produkten har utgått (äldre än 3 år).");
        Console.ResetColor();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Skapa en instans av AssetTracker
        AssetTracker assetTracker = new AssetTracker();

        // Lägg till fler tillgångar (Laptops och Mobiltelefoner)
        assetTracker.AddAsset("Apple", "MacBook", new DateTime(2022, 6, 15), 15000.00m, "Computer");
        assetTracker.AddAsset("Asus", "ZenBook", new DateTime(2021, 1, 15), 12000.00m, "Computer"); // Inköp snart utgången
        assetTracker.AddAsset("Dell", "XPS", new DateTime(2020, 12, 5), 10000.00m, "Computer");
        assetTracker.AddAsset("HP", "EliteBook", new DateTime(2021, 3, 22), 11000.00m, "Computer");
        assetTracker.AddAsset("Lenovo", "ThinkPad", new DateTime(2022, 10, 18), 14000.00m, "Computer");
        assetTracker.AddAsset("Acer", "Aspire", new DateTime(2020, 12, 1), 9000.00m, "Computer");

        // New asset added less than 3 months away from expiration
        assetTracker.AddAsset("Apple", "MacBook Air", new DateTime(2021, 12, 15), 14000.00m, "Computer"); // Less than 3 months to 3 years

        assetTracker.AddAsset("Apple", "iPhone 8", new DateTime(2021, 8, 25), 9999.00m, "Phone");
        assetTracker.AddAsset("Samsung", "Galaxy S21", new DateTime(2021, 7, 18), 7999.00m, "Phone");
        assetTracker.AddAsset("Nokia", "3310", new DateTime(2021, 1, 10), 6000.00m, "Phone"); // Inköp snart utgången
        assetTracker.AddAsset("Asus", "ROG Phone", new DateTime(2020, 4, 15), 8000.00m, "Phone");
        assetTracker.AddAsset("Nothing", "Phone 1", new DateTime(2023, 1, 20), 12000.00m, "Phone");
        assetTracker.AddAsset("Xaomi", "Mi 11", new DateTime(2022, 11, 5), 7000.00m, "Phone");

        // Visa sorterade tillgångar
        assetTracker.DisplayAssetsSorted();
    }
}
