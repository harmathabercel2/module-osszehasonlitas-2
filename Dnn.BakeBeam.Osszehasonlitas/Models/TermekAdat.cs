using System;
using System.Collections.Generic;

public class TermekAdat
{
    public string Termek1Sku { get; set; }
    public string Termek1Bvin { get; set; }
    public string Termek1N { get; set; }         // N�v
    public string Termek1P { get; set; }        // �r
    public string Termek1W { get; set; }         // S�ly
    public string Termek1Kep { get; set; }       // K�p URL
    public string Termek1Meret { get; set; }     // M�ret
    public List<string> egyediTulajdonsagok { get; set; }

}