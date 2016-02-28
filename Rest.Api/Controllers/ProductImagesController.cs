using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Web.Http;


namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductImagesController : ApiController
    {
        // GET api/<controller>/5

        public HttpResponseMessage Get(Guid id, string lang = "en-US")
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                HttpResponseMessage response;

                ProductImage image = (from i in context.ProductImages.AsNoTracking()
                                      where i.ProductGuid == id
                                      select i).FirstOrDefault();

                if (image == null)
                {
                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/resources/defaultProduct" + lang + ".png");
                    response = new HttpResponseMessage { Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    return response;
                }

                string[] images = JsonConvert.DeserializeObject<string[]>(image.ImagesData);

                if (images.Length == 0)
                {
                    return null;
                }
                string firstImage = images[0];
                string[] imageParts = firstImage.Split(';', ',', ':');

                string contentType = imageParts[1];
                string baseType = imageParts[2];
                string imageData = imageParts[3];

                byte[] imageBytes = Convert.FromBase64String(imageData);

                response = new HttpResponseMessage { Content = new StreamContent(new MemoryStream(imageBytes)) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                return response;
            }
        }

        // PUT api/<controller>/5
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public void Put(Guid id, string[] images)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                //save into db
                var productImage = (from i in context.ProductImages
                                    where i.ProductGuid == id
                                    select i).FirstOrDefault();

                if (productImage == null)
                {
                    productImage = new ProductImage { ProductGuid = id };
                    context.ProductImages.Add(productImage);
                }

                productImage.ImagesData = JsonConvert.SerializeObject(images);
                context.SaveChanges();
            }
        }

    }
}