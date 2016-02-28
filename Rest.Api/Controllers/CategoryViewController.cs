using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class CategoryViewController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<CategoryView> Get(string lang = "en-US")
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var categories = (from c in context.Categories
                                  select c).ToList();

                var converted = categories.ConvertAll<CategoryView>(c =>
                {
                    return new CategoryView
                    {
                        Guid = c.Guid,
                        Name = TranslationInfo.ExtractTranslation(c.NameTranslations, lang)
                    };
                });

                return converted;
            }
        }

    }
}