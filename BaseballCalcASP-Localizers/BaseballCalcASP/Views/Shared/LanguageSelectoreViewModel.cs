using System.Globalization;

public class LanguageSelectorViewModel
{
    public IEnumerable<CultureInfo> SupportedCultures { get; set; }
    public CultureInfo CurrentCulture { get; set; }
}
