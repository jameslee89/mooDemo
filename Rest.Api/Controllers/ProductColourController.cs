using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductColourController : ApiController
    {
        // GET api/<controller>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<ProductColourInfo> Get(Guid productGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var list = (from c in context.ProductColours
                            where c.ProductGuid == productGuid
                            select c).ToList();

                var converted = list.ConvertAll(Convert);
                return converted;
            }
        }

        static ProductColourInfo Convert(ProductColour colour)
        {
            return new ProductColourInfo
            {
                Guid = colour.Guid,
                NameTranslations = JsonConvert.DeserializeObject<TranslationInfo[]>(colour.NameTranslations),
                ProductGuid = colour.ProductGuid,
                Value = colour.Value
            };
        }

        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        [Route("~/v1/productColour/{guid}")]
        public ProductColourInfo GetProductColour(Guid guid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var item = (from c in context.ProductColours
                            where c.Guid == guid
                            select c).FirstOrDefault();

                var converted = Convert(item);

                return converted;
            }
        }

        // POST api/<controller>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Post(ProductColourInfo request)
        {
            if (request.ProductGuid == Guid.Empty)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            using (DatabaseContext context = new DatabaseContext())
            {
                ProductColour newColour = new ProductColour
                {
                    Guid = Guid.NewGuid(),
                    NameTranslations = JsonConvert.SerializeObject(request.NameTranslations),
                    Value = request.Value,
                    ProductGuid = request.ProductGuid
                };

                context.ProductColours.Add(newColour);
                context.SaveChanges();
            }
        }

        // POST api/<controller>
        [HttpPut]
        [BasicAuthentication]
        [Route("~/v1/productColour/{guid}")]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid guid, ProductColourInfo request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var item = (from c in context.ProductColours
                            where c.Guid == guid
                            select c).FirstOrDefault();

                if (item == null)
                {
                    return;
                }

                item.NameTranslations = JsonConvert.SerializeObject(request.NameTranslations);
                item.Value = request.Value;
                context.SaveChanges();
            }
        }

        // DELETE api/<controller>/5
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Delete(Guid id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                ProductColour color = (from c in context.ProductColours
                                       where c.Guid == id
                                       select c).FirstOrDefault();

                if (color == null)
                {
                    return;
                }

                context.ProductColours.Remove(color);
                context.SaveChanges();
            }
        }
    }
}