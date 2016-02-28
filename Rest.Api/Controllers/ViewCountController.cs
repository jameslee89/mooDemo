using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ViewCountController : ApiController
    {
        public static object lockObject = new object();
        // GET api/<controller>/5
        public int Get(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                ViewCount count = (from c in context.ViewCounts.AsNoTracking()
                                   where c.Guid == id
                                   select c).FirstOrDefault();


                if (count == null)
                {
                    return 1;
                }

                //include current view as well as previous counts;
                return count.Count + 1;

            }

        }

        // POST api/<controller>
        public void Post(Guid id)
        {
            lock (lockObject)
            {
                using (DatabaseContext context = Util.CreateContext())
                {
                    ViewCount count = (from c in context.ViewCounts
                                       where c.Guid == id
                                       select c).FirstOrDefault();
                    if (count == null)
                    {
                        count = new ViewCount
                        {
                            Guid = id,
                            Count = 0
                        };
                        context.ViewCounts.Add(count);
                    }
                    count.Count++;
                    context.SaveChanges();
                }
            }
        }

    }
}