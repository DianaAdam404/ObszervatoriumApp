using ObszervatoriumApp;

List<Tavcso> tavcsovek = new List<Tavcso>();
int hibasSorok = 0;

string[] sorok = File.ReadAllLines("tavcsovek.txt");

foreach (string sor in sorok.Skip(1))
{
    if (string.IsNullOrWhiteSpace(sor))
    {
        hibasSorok++;
        continue;
    }

    string[] adatok = sor.Split('|');

    if (adatok.Length != 5)
    {
        hibasSorok++;
        continue;
    }

    string tipus = adatok[0];
    string azonosito = adatok[1];
    int atmero = int.Parse(adatok[2]);
    int uzemora = int.Parse(adatok[3]);
    int extra = int.Parse(adatok[4]);

    if (tipus == "O")
    {
        tavcsovek.Add(new Tavcso(azonosito, atmero, uzemora));
    }
    else if (tipus == "R")
    {
        tavcsovek.Add(new Radiotavcso(azonosito, atmero, uzemora, extra));
    }
}

Console.WriteLine($"Sikeresen beolvasott távcsövek: {tavcsovek.Count} db");
Console.WriteLine($"Kiszűrt hibás sorok száma: {hibasSorok} db");
Console.WriteLine();
Console.WriteLine("Beolvasott távcsövek listája:");
foreach (var tavcso in tavcsovek)
{
    Console.WriteLine(tavcso);
}

Console.WriteLine();
Console.WriteLine("LINQ Statisztikák:");

int osszesKoltseg = tavcsovek.Sum(t => t.FenntartasiKoltseg());
Console.WriteLine($"Összesített éves fenntartási költség: {osszesKoltseg} EUR");

double atlagUzemora = tavcsovek.Average(t => t.Uzemora);
Console.WriteLine($"Átlagos üzemóra: {atlagUzemora:F1} óra");

Tavcso? legnagyobbAtmero = tavcsovek.MaxBy(t => t.AtmeroCm);
if (legnagyobbAtmero != null)
{
    Console.WriteLine($"Legnagyobb átmérőjű távcső: {legnagyobbAtmero.Azonosito} ({legnagyobbAtmero.AtmeroCm} cm)");
}

Tavcso? legkisebbKoltseg = tavcsovek.MinBy(t => t.FenntartasiKoltseg());
if (legkisebbKoltseg != null)
{
    Console.WriteLine($"Legalacsonyabb költségű távcső: {legkisebbKoltseg.Azonosito} ({legkisebbKoltseg.FenntartasiKoltseg()} EUR)");
}

int legalabb1000Ora = tavcsovek.Count(t => t.Uzemora >= 1000);
Console.WriteLine($"Legalább 1000 órát üzemelt távcsövek: {legalabb1000Ora} db");

bool van40EzerFelett = tavcsovek.Any(t => t.FenntartasiKoltseg() > 40000);
Console.WriteLine($"Van 40 000 EUR feletti költségű távcső: {(van40EzerFelett ? "Van" : "Nincs")}");

double atlagFrekvencia = tavcsovek.OfType<Radiotavcso>().Average(r => r.FrekvenciaMHz);
Console.WriteLine($"Rádiótávcsövek átlagos frekvenciája: {atlagFrekvencia} MHz");

Console.WriteLine();
Console.WriteLine("Legalább 300 cm átmérőjű távcsövek (csökkenő sorrendben):");
var nagyTavcsovek = tavcsovek
    .Where(t => t.AtmeroCm >= 300)
    .OrderByDescending(t => t.AtmeroCm);

foreach (var t in nagyTavcsovek)
{
    Console.WriteLine($"- {t.Azonosito} ({t.AtmeroCm} cm)");
}

var aktivSorok = tavcsovek
    .Where(t => t.Uzemora > 500)
    .OrderBy(t => t.Uzemora)
    .Select(t => $"{t.Azonosito};{t.AtmeroCm};{t.Uzemora};{t.FenntartasiKoltseg()}");

File.WriteAllLines("aktiv_tavcsovek.txt", aktivSorok);

Console.WriteLine();
Console.WriteLine("Az aktiv_tavcsovek.txt fájl sikeresen elkészült.");
Console.WriteLine($"Távcső példányok száma (static): {Tavcso.OsszesTavcso} db");

