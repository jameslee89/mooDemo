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
    /// <summary>
    /// Category Management
    /// </summary>


    public class CategoryController : ApiController
    {
        //[AcceptVerbs("GET")]
        //[Route("~/v1/category/list")]
        //public IEnumerable<CategoryView> GetList(string lang = "en-US")
        //{
        //    //only show categories of active skus
        //    var skus = new SkuController().GetList(lang: lang).ToList();

        //    var activeCategorieGuids = (from s in skus
        //                                select s.CategoryGuid).Distinct().ToList();

        //    var categoryViews = activeCategorieGuids.ConvertAll((categoryId) =>
        //    {
        //        var match = skus.Find(s => s.CategoryGuid == categoryId);
        //        return new CategoryView
        //        {
        //            Guid = categoryId,
        //            Name = match.CategoryName
        //        };
        //    });

        //    return categoryViews;

        //}
        /// <summary>
        /// Returns category information
        /// </summary>
        /// <returns></returns>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<CategoryInfo> Get()
        {
            List<CategoryInfo> list = new List<CategoryInfo>();

            using (DatabaseContext context = Util.CreateContext())
            {
                var categories = (from c in context.Categories
                                  select c).ToArray();

                foreach (Category category in categories)
                {
                    list.Add(new CategoryInfo
                    {
                        Guid = category.Guid,
                        NameTranslations = JsonConvert.DeserializeObject<TranslationInfo[]>(category.NameTranslations),
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="model"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Post(CategoryInfo model)
        {
            //new, save to database
            Guid guid = Guid.NewGuid();

            Category newCategory = new Category
            {
                Guid = guid,
                NameTranslations = JsonConvert.SerializeObject(model.NameTranslations),
            };

            using (DatabaseContext context = Util.CreateContext())
            {
                context.Categories.Add(newCategory);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid id, CategoryInfo model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var category = (from item in context.Categories
                                where item.Guid == id
                                select item).FirstOrDefault();

                category.NameTranslations = JsonConvert.SerializeObject(model.NameTranslations);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Delete(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                Category category = (from c in context.Categories
                                     where c.Guid == id
                                     select c).FirstOrDefault();

                if (category == null)
                {
                    return;
                }

                context.Categories.Remove(category);
                context.SaveChanges();
            }
        }
    }
}