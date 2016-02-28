using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    /// <summary>
    /// A container for multiple translations of a particular information
    /// </summary>
    public class TranslationInfo
    {
        public string LanguageCode { get; set; }
        public string Text { get; set; }

        public static string ExtractTranslation(string translationInfoJson, string lang)
        {
            if (String.IsNullOrEmpty(translationInfoJson))
            {
                return String.Empty;
            }

            TranslationInfo[] translations = JsonConvert.DeserializeObject<TranslationInfo[]>(translationInfoJson);

            return ExtractTranslation(translations, lang);
        }

        public static string ExtractTranslation(TranslationInfo[] translations, string lang)
        {
            TranslationInfo relevant = (from i in translations
                                        where i.LanguageCode == lang
                                        select i).First();

            return relevant.Text;
        }
    }
}
