using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class MarketLocationController : ApiController
    {
        private static List<MarketLocation> _locations = BuildLocations();

        // GET api/<controller>
        public IEnumerable<MarketLocationView> Get(string lang = "en-US")
        {
            var marketLocationViews = _locations.ConvertAll((ml) =>
            {
                return new MarketLocationView
                {
                    Key = ml.Key,
                    Name = TranslationInfo.ExtractTranslation(ml.Names, lang)
                };
            });
            return marketLocationViews;
        }

        static List<MarketLocation> BuildLocations()
        {
            List<MarketLocation> list = new List<MarketLocation>();
            list.Add(Build("Macau - Macau", "澳門 - 澳門"));
            list.Add(Build("Macau - Taipa", "澳門 - 氹仔"));
            list.Add(Build("Macau - Cotai", "澳門 - 路氹"));
            list.Add(Build("Macau - Coloane", "澳門 - 路環"));
            list.Add(Build("Hong Kong - Hong Kong Island - Central & Western District", "香港 - 香港島 - 	中西區"));
            list.Add(Build("Hong Kong - Hong Kong Island - Eastern District", "香港 - 香港島 - 東區"));
            list.Add(Build("Hong Kong - Hong Kong Island - Southern District", "香港 - 香港島 - 南區"));
            list.Add(Build("Hong Kong - Hong Kong Island - Wan Chai District", "香港 - 香港島 - 灣仔區"));
            list.Add(Build("Hong Kong - Kowloon - Sham Shui Po District", "香港 - 九龍 - 深水埗區"));
            list.Add(Build("Hong Kong - Kowloon - Kowloon City District", "香港 - 九龍 - 九龍城區"));
            list.Add(Build("Hong Kong - Kowloon - Kwun Tong District", "香港 - 九龍 - 觀塘區"));
            list.Add(Build("Hong Kong - Kowloon - Wong Tai Sin District", "香港 - 九龍 - 黃大仙區"));
            list.Add(Build("Hong Kong - Kowloon - Yau Tsim Mong District", "香港 - 九龍 - 油尖旺區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 離島區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 葵青區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 北區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 西貢區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 沙田區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 大埔區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 荃灣區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 屯門區"));
            list.Add(Build("Hong Kong - New Territories - Islands District", "香港 - 新界 - 元朗區"));
            return list;
        }

        static MarketLocation Build(string enName, string zhName)
        {
            TranslationInfo[] info = new TranslationInfo[] { 
                new TranslationInfo{
                     LanguageCode = "en-US",
                      Text = enName
                },
                new TranslationInfo{
                    LanguageCode = "zh-TW",
                     Text = zhName
                }
            };

            //english name is the key for the moment.
            return new MarketLocation
            {
                Key = enName,
                Names = info
            };
        }


    }
}