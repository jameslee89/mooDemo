using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SkuController : ApiController
    {


        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<SkuInfo> Get(Guid productGuid)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var list = (from s in context.Skus.AsNoTracking()
                            where s.ProductGuid == productGuid
                            select s).ToList();

                return list.ConvertAll(sku => new SkuInfo
                {
                    Guid = sku.Guid,
                    ProductGuid = sku.ProductGuid,
                    NameTranslations = JsonConvert.DeserializeObject<TranslationInfo[]>(sku.NameTranslations)
                }); ;
            }
        }

        // POST api/<controller>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Post(SkuInfo model)
        {
            Sku newSku = new Sku
            {
                Guid = Guid.NewGuid(),
                NameTranslations = JsonConvert.SerializeObject(model.NameTranslations),
                ProductGuid = model.ProductGuid
            };

            using (DatabaseContext context = Util.CreateContext())
            {
                context.Skus.Add(newSku);
                context.SaveChanges();
            }
        }

        // PUT api/<controller>/5
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid id, SkuInfo model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {

                //delete and readd
                Sku item = (from i in context.Skus
                            where i.Guid == id
                            select i).FirstOrDefault();

                item.NameTranslations = JsonConvert.SerializeObject(model.NameTranslations);
                context.SaveChanges();
            }
        }

        // DELETE api/<controller>/5
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Delete(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                Sku sku = (from s in context.Skus
                           where s.Guid == id
                           select s).FirstOrDefault();

                if (sku == null)
                {
                    return;
                }
                context.Skus.Remove(sku);
                context.SaveChanges();
            }
        }
    }
}