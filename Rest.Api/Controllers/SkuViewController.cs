using LinkenLabs.Market.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SkuViewController : ApiController
    {

        /// <summary>
        /// Main SKU search entry point
        /// </summary>
        /// <param name="query"></param>
        /// <param name="categories"></param>
        /// <param name="manufacturers"></param>
        /// <param name="skus"></param>
        /// <param name="showInactive">Whether or not to show skus with no orders</param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("~/v1/skuView")]
        public IEnumerable<SkuView> GetList(string query = "", string categories = "", string manufacturers = "", string skus = "", bool showInactive = true, string lang = "en-US")
        {
            var list = GenerateAllSkuViews(lang);

            if (!String.IsNullOrEmpty(query))
            {
                list = list.Where((sku) =>
                {
                    string[] queryParts = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in queryParts)
                    {
                        if (sku.Keywords.IndexOf(item, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            return false;
                        }
                    }
                    return true;
                });
            }

            if (!String.IsNullOrEmpty(categories))
            {
                List<Guid> categoriesGuids = categories.Split(',').ToList().ConvertAll(p => Guid.Parse(p));
                list = list.Where((sku) => categoriesGuids.Contains(sku.CategoryGuid));
            }

            if (!String.IsNullOrEmpty(manufacturers))
            {
                List<Guid> manufacturersGuids = manufacturers.Split(',').ToList().ConvertAll(p => Guid.Parse(p));
                list = list.Where((sku) => manufacturersGuids.Contains(sku.ManufacturerGuid));
            }

            if (!String.IsNullOrEmpty(skus))
            {
                List<Guid> skuGuids = skus.Split(',').ToList().ConvertAll(p => Guid.Parse(p));
                list = list.Where((sku) => skuGuids.Contains(sku.Guid));
            }

            if (!showInactive)
            {
                list = list.Where((sku) => sku.OrdersCount > 0);
            }

            return list;
        }

        static IEnumerable<SkuView> GenerateAllSkuViews(string lang)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var allAccounts = context.Database.SqlQuery<Account>(@"SELECT * FROM Accounts").ToList();
                var allSkus = context.Database.SqlQuery<Sku>(@"SELECT * FROM Skus").ToList();
                var allManufacturers = context.Database.SqlQuery<Manufacturer>(@"SELECT * FROM Manufacturers").ToList();
                var allProducts = context.Database.SqlQuery<Product>(@"SELECT * FROM Products").ToList();
                var allCategories = context.Database.SqlQuery<Category>(@"SELECT * FROM Categories").ToList();
                var allProductCategories = context.Database.SqlQuery<ProductCategory>(@"SELECT * FROM ProductCategories").ToList();
                var allOrders = context.Database.SqlQuery<Order>(@"SELECT * FROM Orders").ToList();
                var allTrades = context.Database.SqlQuery<Trade>(@"SELECT * FROM Trades").ToList();
                var allViewCounts = context.ViewCounts.AsNoTracking().ToList();

                var baseUrl = WebConfigurationManager.AppSettings["BaseUrl"];
                var webUrl = WebConfigurationManager.AppSettings["WebUrl"];

                var skuViews = allSkus.ConvertAll(sku =>
                {
                    var product = allProducts.Find(p => p.Guid == sku.ProductGuid);
                    var manufacturer = allManufacturers.Find(m => m.Guid == product.ManufacturerGuid);
                    var productCategory = allProductCategories.Find(pc => pc.ProductGuid == sku.ProductGuid);
                    var category = allCategories.Find(c => c.Guid == productCategory.CategoryGuid);
                    var viewCount = allViewCounts.Find(c => c.Guid == sku.Guid);

                    var skuView = new SkuView
                    {
                        CategoryGuid = category.Guid,
                        CategoryName = TranslationInfo.ExtractTranslation(category.NameTranslations, lang),
                        ProductGuid = product.Guid,
                        ProductName = TranslationInfo.ExtractTranslation(product.NameTranslations, lang),
                        ProductDescription = TranslationInfo.ExtractTranslation(product.DescriptionTranslations, lang),
                        ManufacturerGuid = manufacturer.Guid,
                        ManufacturerName = TranslationInfo.ExtractTranslation(manufacturer.NameTranslations, lang),
                        Guid = sku.Guid,
                        SkuName = TranslationInfo.ExtractTranslation(sku.NameTranslations, lang),
                        ImageSrc = String.Format("{0}/v1/productimages/{1}?lang={2}", baseUrl, product.Guid, lang),
                        Url = String.Format("{0}/{1}/market/{2}", webUrl, lang, sku.Guid),
                        ManufacturerLink = product.ManufacturerLink,
                        ViewCount = viewCount == null ? 0 : viewCount.Count
                    };

                    string[] fullNameParts = skuView.SkuName == "null" ? new string[] { skuView.ManufacturerName, skuView.ProductName } :
                        new string[] { skuView.ManufacturerName, skuView.ProductName, skuView.SkuName };
                    skuView.FullName = String.Join(" ", fullNameParts);

                    Func<string, string[]> getValues = (nameTranslations) =>
                    {
                        var translations = JsonConvert.DeserializeObject<TranslationInfo[]>(nameTranslations);
                        return (from t in translations select t.Text == "null" ? "" : t.Text).ToArray();
                    };

                    List<string> keywordParts = new List<string>();
                    keywordParts.AddRange(getValues(category.NameTranslations));
                    keywordParts.AddRange(getValues(manufacturer.NameTranslations));
                    keywordParts.AddRange(getValues(product.NameTranslations));
                    keywordParts.AddRange(getValues(sku.NameTranslations));
                    skuView.Keywords = String.Join(" ", keywordParts.Distinct());

                    ////build price low only for active orders
                    var results = (from o in allOrders
                                   where o.SkuGuid == sku.Guid
                                   && !o.IsCancelled
                                   && o.Quantity > 0
                                   && o.ValidFrom < DateTime.UtcNow
                                    && o.ValidTo > DateTime.UtcNow
                                   select o).ToList();

                    skuView.OrdersCount = results.Count;

                    //only summarise sell order pricing
                    var sellOrders = results.Where(o => o.Type == "Sell").ToList();

                    if (sellOrders.Count > 0)
                    {
                        var cheapest = sellOrders.OrderBy(o => o.Price).First();
                        skuView.PriceLow = cheapest.Price;
                        skuView.IsExclusive = cheapest.IsExclusive;
                        skuView.Currency = cheapest.Currency;

                        var latest = (from order in results
                                      orderby order.LastModified descending
                                      select order).First();
                        skuView.LastModified = latest.LastModified;
                    }

                    skuView.SoldCount = (from t in allTrades
                                         where t.SkuGuid == sku.Guid
                                         select t.Quantity).Sum();

                    return skuView;
                });

                return skuViews;

            }
        }

        // GET api/<controller>/5
        [Route("~/v1/skuView/{id}")]
        public SkuView Get(Guid id, string lang = "en-US")
        {
            var list = GetList(showInactive: true, lang: lang);

            SkuView skuView = (from item in list
                               where item.Guid == id
                               select item).FirstOrDefault();

            if (skuView == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return skuView;

        }
    }
}