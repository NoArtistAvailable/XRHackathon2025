using elZach.Common;
using TMPro;
using UnityEngine;

public class ShowQuote : MonoBehaviour
{
    public static string[] quotes = new string[]{
        "Keine Grenze zwischen Künstler und Handwerker",
        "Der Künstler ist ein Begeisterer",
        "Die Form folgt der Funktion",
        "Gesamtkunstwerk",
        "Echte Materialien sollten die wahre Natur von Objekten und Gebäuden widerspiegeln",
        "Die Bauhauskünstler bevorzugten lineare und geometrische Formen.",
        "Betont die Technik",
        "Intelligente Nutzung von Ressourcen",
        "Einfachheit und Effektivität",
        "Ständige Entwicklung"
    };
    void OnEnable()
    {
        var tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (!tmp) return;
        tmp.text = quotes.GetRandom();
    }
}
