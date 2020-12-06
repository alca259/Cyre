using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cyre.Api.Kernel
{
    public class LocalizationConfiguration
    {
        public string DefaultLanguage { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }

        internal static LocalizationConfiguration BuildDefault()
        {
            return new LocalizationConfiguration
            {
                DefaultLanguage = "es-ES",
                SupportedLanguages = new List<string> { "en-US", "es-ES" },
                SupportedCultures = new List<string> { "en-US", "es-ES" }.Select(x => new CultureInfo(x)).ToList()
            };
        }
    }
}
