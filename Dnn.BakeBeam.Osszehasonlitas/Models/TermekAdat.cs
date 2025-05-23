using System;
using System.Collections.Generic;

public class TermekAdat
{
    public string Termek1Sku { get; set; }
    public string Termek1Bvin { get; set; }
    public string Termek1N { get; set; }         // Név
    public string Termek1P { get; set; }        // Ár
    public string Termek1W { get; set; }         // Súly
    public string Termek1Kep { get; set; }       // Kép URL
    public string Termek1Meret { get; set; }     // Méret
    public List<string> egyediTulajdonsagok { get; set; }

}