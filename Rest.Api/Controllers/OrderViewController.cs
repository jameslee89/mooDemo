using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class OrderViewController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="skuId"></param>
        /// <param name="orderId"></param>
        /// <param name="query"></param>
        /// <param name="categories"></param>
        /// <param name="manufacturers"></param>
        /// <param name="lang"></param>
        /// <param name="showInactive">whether to show orders from inactive stores</param>
        /// <returns></returns>
        [Route("~/v1/orderView")]
        public IEnumerable<OrderView> Get(string accountGuid = "", string skuId = "", string orderId = "", string type = "", string query = "", string categories = "", string manufacturers = "", string lang = "en-US", bool showExpired = false, bool showSoldOut = false, bool showCancelled = false)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var activeAccounts = new AccountController().GetAccounts().ToList();
                var relevantSkuViews = new SkuViewController().GetList(query, categories, manufacturers, skuId, true, lang).ToList();


                var orders = (from o in context.Orders.AsNoTracking()
                              select o).ToList();

                //only show orders which match sku filters

                var relevantSkuGuids = relevantSkuViews.ConvertAll(s => s.Guid);
                orders = orders.Where(o => relevantSkuGuids.Contains(o.SkuGuid)).ToList();

                //store filter
                if (!String.IsNullOrEmpty(accountGuid))
                {
                    orders = orders.Where(o =>
                    {
                        return o.CreatedByAccountGuid == new Guid(accountGuid);
                    }).ToList();
                }

                if (!String.IsNullOrEmpty(type))
                {
                    orders = orders.Where(o => o.Type == type).ToList();
                }

                if (!String.IsNullOrEmpty(skuId))
                {
                    orders = orders.Where(o => o.SkuGuid == new Guid(skuId)).ToList();
                }

                if (!String.IsNullOrEmpty(orderId))
                {
                    orders = orders.Where(o => o.Guid == new Guid(orderId)).ToList();
                }

                if (!showExpired)
                {
                    orders = orders.Where(o => o.ValidTo > DateTime.UtcNow).ToList();
                }

                if (!showSoldOut)
                {
                    orders = (from order in orders
                              where activeAccounts.Any(a => a.Guid == order.CreatedByAccountGuid)
                              && order.Quantity > 0
                              select order).ToList();
                }

                if (!showCancelled)
                {
                    orders = (from order in orders
                              where !order.IsCancelled
                              select order).ToList();
                }

                var webUrl = ConfigurationManager.AppSettings["WebUrl"];
                var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

                var marketLocations = new MarketLocationController().Get(lang).ToList();
                var productConditions = new ProductConditionController().Get(lang).ToList();
                var productColours = (from c in context.ProductColours.AsNoTracking()
                                      select c).ToList();

                var orderViews = orders.ConvertAll(o =>
                {
                    var sku = relevantSkuViews.Where(s => s.Guid == o.SkuGuid).FirstOrDefault();
                    var account = activeAccounts.Where(a => a.Guid == o.CreatedByAccountGuid).FirstOrDefault();
                    var marketLocation = marketLocations.Where(l => l.Key == o.Location).FirstOrDefault();
                    var isActive = DateTime.UtcNow > o.ValidFrom 
                                    && DateTime.UtcNow < o.ValidTo 
                                    && o.Quantity > 0
                                    && !o.IsCancelled;
                    var status = o.IsCancelled ? "Cancelled" : o.Quantity == 0 ? "Sold out" : DateTime.UtcNow > o.ValidTo ? "Expired" : "Active";
                    var viewCount = (from c in context.ViewCounts
                                     where c.Guid == o.Guid
                                     select c).FirstOrDefault();

                    var productCondition = productConditions.Where(c => c.Key == o.ProductCondition).FirstOrDefault();
                    var productColour = (from c in productColours
                                         where c.Guid == o.ProductColourGuid
                                         select c).FirstOrDefault();
                    var imageGuids = JsonConvert.DeserializeObject<Guid[]>(o.ImageGuids).ToList();
                    var imageUrls = imageGuids.ConvertAll(guid => String.Format("{0}/v1/image/{1}", baseUrl, guid));

                    var orderView = new OrderView
                    {
                        Price = o.Price,
                        Currency = o.Currency,
                        Quantity = o.Quantity,
                        QuantityInitial = o.QuantityInitial,
                        MinimumOrder = o.MinimumOrder,
                        IsExclusive = o.IsExclusive,
                        Guid = o.Guid,
                        SkuGuid = o.SkuGuid,
                        CreatedByAccountGuid = o.CreatedByAccountGuid,
                        AccountUserName = account.Username,
                        AccountPhone = account.Phone,
                        Location = marketLocation.Key,
                        LocationName = marketLocation.Name,
                        ProductColourGuid = o.ProductColourGuid,
                        ProductColourName = TranslationInfo.ExtractTranslation(productColour.NameTranslations, lang),
                        ProductColourValue = productColour.Value,
                        ProductCondition = o.ProductCondition,
                        ProductConditionName = productCondition.Name,
                        Type = o.Type,
                        Created = DateTime.SpecifyKind(o.Created, DateTimeKind.Utc),
                        ValidFrom = DateTime.SpecifyKind(o.ValidFrom, DateTimeKind.Utc),
                        ValidTo = DateTime.SpecifyKind(o.ValidTo, DateTimeKind.Utc),
                        SkuName = sku.SkuName,
                        SkuFullName = sku.FullName,
                        CategoryGuid = sku.CategoryGuid,
                        CategoryName = sku.CategoryName,
                        ManufacturerGuid = sku.ManufacturerGuid,
                        ManufacturerName = sku.ManufacturerName,
                        ProductGuid = sku.ProductGuid,
                        ProductName = sku.ProductName,
                        LastModified = DateTime.SpecifyKind(o.LastModified, DateTimeKind.Utc),
                        ImageSrc = sku.ImageSrc,
                        Description = o.Description,
                        IsActive = isActive,
                        Status = status,
                        Url = String.Format("{0}/{1}/order/{2}", webUrl, lang, o.Guid),
                        ViewCount = viewCount != null ? viewCount.Count : 0,
                        ImageUrls = imageUrls.ToArray()
                    };

                    return orderView;
                });
                return orderViews;
            }
        }

        [Route("~/v1/orderView/{id}")]
        public OrderView Get(Guid id, string lang = "en-US")
        {
            var result = Get(orderId: id.ToString(), lang: lang, showExpired: true, showCancelled: true, showSoldOut: true).FirstOrDefault();
            return result;
        }

    }
}