using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class OrderController : ApiController
    {

        static bool IsValidImage(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    System.Drawing.Image.FromStream(ms);
                }

            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
        // POST api/<controller>
        [BasicAuthentication]
        public void Post(OrderRequest model)
        {
            Account currentAccount = AccountController.GetAccountByUsername(User.Identity.Name);

            List<Image> images = new List<Image>();
            //check images
            foreach (var imageData in model.Images)
            {
                string[] imageParts = imageData.Split(';', ',', ':');

                string contentType = imageParts[1];
                string baseType = imageParts[2];
                string data = imageParts[3];

                byte[] imageBytes = Convert.FromBase64String(data);

                if (!IsValidImage(imageBytes))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                images.Add(new Image
                {
                    Guid = Guid.NewGuid(),
                    ContentType = contentType,
                    Data = imageBytes
                });
            }

            using (DatabaseContext context = Util.CreateContext())
            {
                //check location exists
                var locations = new MarketLocationController().Get().ToList();
                var conditions = new ProductConditionController().Get().ToList();
                var selectedColour = (from c in context.ProductColours
                                     where c.Guid == model.ProductColourGuid
                                     select c).FirstOrDefault();
 
                var selectedLocation = (from l in locations
                                        where l.Key == model.Location
                                        select l).FirstOrDefault();
                var selectedCondition = (from c in conditions
                                         where c.Key == model.ProductCondition
                                         select c).FirstOrDefault();

                if (selectedLocation == null
                    || selectedCondition == null
                    || selectedColour == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                context.Images.AddRange(images);

                Order newOrder = new Order
                {
                    Guid = Guid.NewGuid(),
                    CreatedByAccountGuid = currentAccount.Guid,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    Currency = model.Currency,
                    Price = model.Price,
                    SkuGuid = model.SkuGuid,
                    Location = selectedLocation.Key,
                    ProductCondition = selectedCondition.Key,
                    ProductColourGuid = model.ProductColourGuid,
                    Type = model.Type,
                    Quantity = model.Quantity,
                    QuantityInitial = model.Quantity,
                    MinimumOrder = model.MinimumOrder,
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddDays(model.DurationDays),
                    Description = model.Description,
                    ImageGuids = JsonConvert.SerializeObject(images.ConvertAll(i => i.Guid).ToArray())
                };

                context.Orders.Add(newOrder);
                context.SaveChanges();
            }
        }

        [Route("~/v1/order/{orderGuid}")]
        [BasicAuthentication]
        [HttpPatch]
        public void Patch(Guid orderGuid, dynamic model)
        {
            if (!IsAuthorized(orderGuid))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            using (DatabaseContext context = new DatabaseContext())
            {
                Order order = (from o in context.Orders
                               where o.Guid == orderGuid
                               select o).FirstOrDefault();

                if (order == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (model["Price"] == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                order.Price = model.Price;
                order.LastModified = DateTime.UtcNow;
                context.SaveChanges();
            }
        }

        bool IsAuthorized(Guid orderGuid)
        {
            Order order = GetOrderByGuid(orderGuid);
            Account userAccount = AccountController.GetAccountByUsername(User.Identity.Name);

            if (order.CreatedByAccountGuid == userAccount.Guid)
            {
                return true;
            }
            return false;
        }

        Order GetOrderByGuid(Guid orderGuid)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var order = (from o in context.Orders
                             where o.Guid == orderGuid
                             select o).FirstOrDefault();
                return order;
            }
        }

        // DELETE api/<controller>/5
        [BasicAuthentication]
        [Route("~/v1/order/{orderGuid}")]
        [HttpDelete]
        public void Delete(Guid orderGuid)
        {
            if (!IsAuthorized(orderGuid))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            Account userAccount = AccountController.GetAccountByUsername(User.Identity.Name);

            using (DatabaseContext context = Util.CreateContext())
            {
                var order = (from o in context.Orders
                             where o.Guid == orderGuid
                             select o).FirstOrDefault();

                if (order == null)
                {
                    return;
                }

                order.IsCancelled = true;
                order.LastModified = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}