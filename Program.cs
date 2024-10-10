using System;
using System.Collections.Generic;
using System.Linq;

class Asset
{
    public string ModelName { get; set; }       // Modellens namn
    public DateTime PurchaseDate { get; set; }  // Inköpsdatum
    public decimal Price { get; set; }          // Pris i SEK
    public string AssetType { get; set; }       // Typ av tillgång (t.ex. dator, telefon)
    public string Brand { get; set; }           // Märke
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

    // Kolla om tillgången är mindre än 3 månader från att utgå (markerad röd)
    public bool IsAboutToExpireRed() => (EndOfLifeDate - DateTime.Now).TotalDays <= 90 && (EndOfLifeDate - DateTime.Now).TotalDays >= 0;

    // Kolla om tillgången är mindre än 6 månader men mer än 3 månader från att utgå (markerad gul)
    public bool IsAboutToExpireYellow() => (EndOfLifeDate - DateTime.Now).TotalDays <= 180 && (EndOfLifeDate - DateTime.Now).TotalDays > 90;

    // Kolla om tillgången har mer än 6 månader kvar (markerad grön)
    public bool HasMoreThan6Months() => (EndOfLifeDate - DateTime.Now).TotalDays > 180;

    // Kolla om tillgången redan har utgått (äldre än 3 år)
    public bool IsExpired() => DateTime.Now > EndOfLifeDate;

    // Metod för att visa information om tillgången med färgkod
    public void DisplayInfo()
    {
        if (IsExpired())
        {
            Console.ForegroundColor = ConsoleColor.Gray; // Markera utgångna tillgångar i grått
            Console.WriteLine($"{AssetType,-10} {Brand,-15} {ModelName,-20} {Price,10} SEK {PurchaseDate.ToShortDateString()}");
        }
        else if (IsAboutToExpireRed())
        {
            Console.ForegroundColor = ConsoleColor.Red; // Markera tillgångar som är mindre än 3 månader från att utgå i rött
            Console.WriteLine($"{AssetType,-10} {Brand,-15} {ModelName,-20} {Price,10} SEK {PurchaseDate.ToShortDateString()}");
        }
        else if (IsAboutToExpireYellow())
        {
            Console.ForegroundColor = ConsoleColor.Yellow; // Markera tillgångar som är mellan 3 och 6 månader från att utgå i gult
            Console.WriteLine($"{AssetType,-10} {Brand,-15} {ModelName,-20} {Price,10} SEK {PurchaseDate.ToShortDateString()}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green; // Markera tillgångar som har mer än 6 månader kvar i grönt
            Console.WriteLine($"{AssetType,-10} {Brand,-15} {ModelName,-20} {Price,10} SEK {PurchaseDate.ToShortDateString()}");
        }

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
        // Sortering: först efter typ (datorer först), sedan efter färgkod (grön, gul, röd)
        var sortedAssets = assets
            .OrderBy(asset => asset.AssetType) // Sortera efter typ (datorer först, telefoner sen)
            .ThenByDescending(asset => asset.HasMoreThan6Months()) // Grön först
            .ThenByDescending(asset => asset.IsAboutToExpireYellow()) // Gul sedan
            .ThenByDescending(asset => asset.IsAboutToExpireRed()) // Röd sist
            .ThenBy(asset => asset.PurchaseDate); // Slutligen sortera efter inköpsdatum inom varje färgkod

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
        Console.WriteLine("GRÖN   : Tillgången har mer än 6 månader kvar till utgång.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("GUL    : Tillgången har mindre än 6 månader men mer än 3 månader kvar till utgång.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("RÖD    : Tillgången har mindre än 3 månader kvar till utgång.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("GRÅ    : Tillgången har passerat 3 år och är utgången.");
        Console.ResetColor();
    }
}

class Program
{
    static void Main(string[] args)
    {
        AssetTracker assetTracker = new AssetTracker();

        // Lägger till fler laptops och telefoner som faller in i olika intervall (grön, gul, röd, utgången)

        // Laptops
        assetTracker.AddAsset("HP", "Elitebook", new DateTime(2022, 11, 15), 12000m, "Dator");
        assetTracker.AddAsset("Dell", "XPS 13", new DateTime(2020, 5, 25), 15000m, "Dator");
        assetTracker.AddAsset("Lenovo", "ThinkPad", new DateTime(2020, 4, 1), 14000m, "Dator");
        assetTracker.AddAsset("Asus", "Zenbook", new DateTime(2023, 1, 20), 14000m, "Dator");
        assetTracker.AddAsset("Acer", "Aspire 5", new DateTime(2021, 12, 1), 10000m, "Dator");
        assetTracker.AddAsset("Apple", "MacBook Air", new DateTime(2021, 7, 1), 18000m, "Dator");
        assetTracker.AddAsset("MSI", "Prestige 14", new DateTime(2021, 6, 5), 21000m, "Dator");
        // Telefoner
        assetTracker.AddAsset("Iphone", "13 Pro", new DateTime(2021, 9, 1), 13000m, "Telefon");
        assetTracker.AddAsset("Samsung", "Galaxy S21", new DateTime(2022, 7, 5), 10500m, "Telefon");
        assetTracker.AddAsset("Nokia", "G50", new DateTime(2021, 10, 20), 4000m, "Telefon");
        assetTracker.AddAsset("ROG", "Phone 5", new DateTime(2021, 2, 10), 8500m, "Telefon");
        assetTracker.AddAsset("Nothing", "Phone 1", new DateTime(2022, 11, 10), 8000m, "Telefon");
        assetTracker.AddAsset("Iphone", "8", new DateTime(2019, 10, 10), 7000m, "Telefon");

        // Visa sorterade tillgångar
        assetTracker.DisplayAssetsSorted();
    }
}
