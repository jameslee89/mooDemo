using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ImageController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get(Guid id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Image image = (from i in context.Images.AsNoTracking()
                               where i.Guid == id
                               select i).FirstOrDefault();

                if (image == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                HttpResponseMessage response = new HttpResponseMessage { 
                    Content = new StreamContent(new MemoryStream(image.Data)) 
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                return response;
            }
        }
    }
}