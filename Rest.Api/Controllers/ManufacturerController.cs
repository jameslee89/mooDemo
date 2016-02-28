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
    /// Provides management of manufacturers
    /// </summary>
    public class ManufacturerController : ApiController
    {
        /// <summary>
        /// Get manufacturers
        /// </summary>
        /// <returns>Array of manufacturers</returns>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<ManufacturerInfo> Get()
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var manufacturers = (from m in context.Manufacturers
                                     select m).ToList();

                return manufacturers.ConvertAll<ManufacturerInfo>((m) =>
                {
                    return new ManufacturerInfo
                    {
                        Guid = m.Guid,
                        NameTranslations = JsonConvert.DeserializeObject<TranslationInfo[]>(m.NameTranslations)
                    };
                });
            }
        }

        /// <summary>
        /// Creates a new manufacturer
        /// </summary>
        /// <param name="model">data for the new manufacturer</param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Post(ManufacturerInfo model)
        {
            //new, save to database
            Guid guid = Guid.NewGuid();
            Guid nameGuid = Guid.NewGuid();

            Manufacturer newManufacturer = new Manufacturer
            {
                Guid = guid,
                NameTranslations = JsonConvert.SerializeObject(model.NameTranslations)
            };

            using (DatabaseContext context = Util.CreateContext())
            {
                context.Manufacturers.Add(newManufacturer);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a manufacturer
        /// </summary>
        /// <param name="id">id of the manufacturer</param>
        /// <param name="model">data used to replace manufacturer</param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid id, ManufacturerInfo model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                //delete and readd
                var manufacturer = (from m in context.Manufacturers
                                    where m.Guid == id
                                    select m).First();
                manufacturer.NameTranslations = JsonConvert.SerializeObject(model.NameTranslations);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Delets a manufacturer by id
        /// </summary>
        /// <param name="id"></param>
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Delete(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                Manufacturer manufacturer = (from m in context.Manufacturers
                                             where m.Guid == id
                                             select m).FirstOrDefault();

                if (manufacturer == null)
                {
                    return;
                }

                context.Manufacturers.Remove(manufacturer);
                context.SaveChanges();
            }

        }
    }
}