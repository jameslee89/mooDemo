using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductConditionController : ApiController
    {
        private static List<ProductCondition> _list = BuildProductConditions();
        // GET api/<controller>
        public IEnumerable<ProductConditionView> Get(string lang = "en-US")
        {
            var productConditionViews = _list.ConvertAll((pc) =>
            {
                return new ProductConditionView
                {
                    Key = pc.Key,
                    Name = TranslationInfo.ExtractTranslation(pc.Names, lang)
                };
            });
            return productConditionViews;
        }

        static List<ProductCondition> BuildProductConditions()
        {
            List<ProductCondition> list = new List<ProductCondition>();
            list.Add(Build("New", "全新"));
            list.Add(Build("Used", "二手"));
            list.Add(Build("Broken", "破碎"));
            return list;
        }

        static ProductCondition Build(string enName, string zhName)
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
            return new ProductCondition
            {
                Key = enName,
                Names = info
            };
        }
    }
}