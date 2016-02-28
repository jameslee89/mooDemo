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
    public class ProductController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerGuid"></param>
        /// <returns></returns>
        public IEnumerable<ProductInfo> Get(Guid? manufacturerGuid = null)
        {
            Func<Product, bool> predicate = (prop) => true;

            if (manufacturerGuid.HasValue)
            {
                predicate = p => p.ManufacturerGuid == manufacturerGuid;
            }

            List<ProductInfo> list = new List<ProductInfo>();

            using (DatabaseContext context = Util.CreateContext())
            {
                var relatedProducts = (from p in context.Products.AsNoTracking()
                                       select p).Where(predicate).ToArray();

                foreach (Product item in relatedProducts)
                {
                    //convert to productInfo
                    var relatedCategory = (from c in context.ProductCategories.AsNoTracking()
                                           where c.ProductGuid == item.Guid
                                           select c).FirstOrDefault();

                    List<string> supportedLanguages = new LanguageController().Get().ToList();
                    TranslationInfo[] emptyValues = supportedLanguages.ConvertAll(lang => new TranslationInfo
                    {
                        LanguageCode = lang
                    }).ToArray();

                    ProductInfo info = new ProductInfo
                    {
                        Guid = item.Guid,
                        ManufacturerGuid = item.ManufacturerGuid,
                        CategoryGuid = relatedCategory != null ? relatedCategory.CategoryGuid : default(Guid),
                        NameTranslations = JsonConvert.DeserializeObject<TranslationInfo[]>(item.NameTranslations),
                        DescriptionTranslations = String.IsNullOrEmpty(item.DescriptionTranslations) ? emptyValues : JsonConvert.DeserializeObject<TranslationInfo[]>(item.DescriptionTranslations),
                        ManufacturerLink = item.ManufacturerLink
                    };

                    if (!info.CategoryGuid.Equals(Guid.Empty))
                    {
                        var cat = (from c in context.Categories
                                   where c.Guid == info.CategoryGuid
                                   select c).First();

                        var name = (from t in JsonConvert.DeserializeObject<TranslationInfo[]>(cat.NameTranslations)
                                    where t.LanguageCode == "en-US"
                                    select t).First().Text;
                        info.CategoryName = name;
                    }

                    list.Add(info);
                }

                return list;
            }


        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="model"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Post(ProductInfo model)
        {
            if (model.ManufacturerGuid == Guid.Empty)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Guid guid = Guid.NewGuid();

            Product newProduct = new Product
            {
                Guid = guid,
                ManufacturerGuid = model.ManufacturerGuid,
                ManufacturerLink = model.ManufacturerLink,
                NameTranslations = JsonConvert.SerializeObject(model.NameTranslations),
                DescriptionTranslations = JsonConvert.SerializeObject(model.DescriptionTranslations)
            };

            using (DatabaseContext context = Util.CreateContext())
            {
                context.Products.Add(newProduct);


                if (model.CategoryGuid != Guid.Empty)
                {
                    context.ProductCategories.Add(new ProductCategory
                    {
                        ProductGuid = newProduct.Guid,
                        CategoryGuid = model.CategoryGuid
                    });
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid id, ProductInfo model)
        {
            Delete(id);
            using (DatabaseContext context = Util.CreateContext())
            {
                Product product = new Product
                {
                    Guid = id,
                    ManufacturerGuid = model.ManufacturerGuid,
                    NameTranslations = JsonConvert.SerializeObject(model.NameTranslations),
                    DescriptionTranslations = JsonConvert.SerializeObject(model.DescriptionTranslations),
                    ManufacturerLink = model.ManufacturerLink
                };

                context.Products.Add(product);

                if (model.CategoryGuid != Guid.Empty)
                {
                    context.ProductCategories.Add(new ProductCategory
                    {
                        ProductGuid = model.Guid,
                        CategoryGuid = model.CategoryGuid
                    });
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Delete(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                Product product = (from p in context.Products
                                   where p.Guid == id
                                   select p).FirstOrDefault();

                if (product == null)
                {
                    return;
                }

                context.Products.Remove(product);

                //categories
                var relatedCategories = (from c in context.ProductCategories
                                         where c.ProductGuid == id
                                         select c).ToArray();
                context.ProductCategories.RemoveRange(relatedCategories);

                context.SaveChanges();
            }
        }

    }
}