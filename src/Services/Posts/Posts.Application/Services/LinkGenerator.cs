using System.Text;
using System.Text.RegularExpressions;

namespace Posts.Application.Services;

public class LinkGenerator
{
    private static readonly Regex CyrillicRegex = new("[\\u0400-\\u04FF]");

    private static readonly Func<string, string> DefaultTransform = s =>
    {
        var lower = s.ToLowerInvariant();
        var cleaned = Regex.Replace(lower, @"[^a-z0-9\s-]", "");
        var withoutWhitespaces = Regex.Replace(cleaned, @"[\s-]", "-");
        return withoutWhitespaces;
    };
    
    public string Generate(string metadata)
    {
        if (CyrillicRegex.IsMatch(metadata))
        {
            return InternalGenerate(metadata, s =>
            {
                string lower = s.ToLowerInvariant();
                string cleaned = Regex.Replace(lower, @"[^а-я0-9\s-]", "");
                string transliterated = Transliterate(cleaned);
                var withoutWhitespaces = Regex.Replace(transliterated, @"[\s-]", "-");
                return withoutWhitespaces;
            });
        }

        return InternalGenerate(metadata);
    }

    private static string InternalGenerate(string metadata, Func<string, string>? transform = null)
    {
        transform ??= DefaultTransform;
        
        var newString = transform(metadata);
        
        string result = Regex.Replace(newString, @"[\s-]+", "-");
        
        result = result.Trim('-');

        return result;
    }

    private static string Transliterate(string text)
    {
        var sb = new StringBuilder();
        
        foreach (var c in text)
        {
            switch (c)
            {
                case 'а': sb.Append('a'); break;
                case 'б': sb.Append('b'); break;
                case 'в': sb.Append('v'); break;
                case 'г': sb.Append('g'); break;
                case 'д': sb.Append('d'); break;
                case 'е': sb.Append('e'); break;
                case 'ё': sb.Append("yo"); break;
                case 'ж': sb.Append("zh"); break;
                case 'з': sb.Append('z'); break;
                case 'и': sb.Append('i'); break;
                case 'й': sb.Append('y'); break;
                case 'к': sb.Append('k'); break;
                case 'л': sb.Append('l'); break;
                case 'м': sb.Append('m'); break;
                case 'н': sb.Append('n'); break;
                case 'о': sb.Append('o'); break;
                case 'п': sb.Append('p'); break;
                case 'р': sb.Append('r'); break;
                case 'с': sb.Append('s'); break;
                case 'т': sb.Append('t'); break;
                case 'у': sb.Append('u'); break;
                case 'ф': sb.Append('f'); break;
                case 'х': sb.Append('h'); break;
                case 'ц': sb.Append('c'); break;
                case 'ч': sb.Append("ch"); break;
                case 'ш': sb.Append("sh"); break;
                case 'щ': sb.Append("sh"); break;
                case 'ъ': sb.Append(""); break;
                case 'ы': sb.Append('y'); break;
                case 'ь': sb.Append(""); break;
                case 'э': sb.Append('e'); break;
                case 'ю': sb.Append("yu"); break;
                case 'я': sb.Append("ya"); break;
                default: sb.Append(c); break;
            }
        }

        return sb.ToString();
    }
}