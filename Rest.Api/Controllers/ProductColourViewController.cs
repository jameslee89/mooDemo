using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductColourViewController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<ProductColourView> Get(Guid productGuid, string lang = "en-US")
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var results = (from c in context.ProductColours
                               where c.ProductGuid == productGuid
                               select c).ToList();

                var converted = results.ConvertAll<ProductColourView>(c =>
                {
                    return new ProductColourView
                    {
                        Guid = c.Guid,
                        ProductGuid = c.ProductGuid,
                        Name = TranslationInfo.ExtractTranslation(c.NameTranslations, lang),
                        Value = c.Value
                    };
                });

                return converted;
            }
        }
    }
}