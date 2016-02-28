using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class CommentViewController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<CommentView> Get(Guid orderGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var comments = (from c in context.Comments
                                where c.OrderGuid == orderGuid
                                select c).ToList();

                var order = (from o in context.Orders
                             where o.Guid == orderGuid
                             select o).First();

                var converted = comments.ConvertAll<CommentView>((c) =>
                {
                    var creator = (from a in context.Accounts
                                   where a.Guid == c.CreatedBy
                                   select a).First();

                    return new CommentView
                    {
                        Guid = c.Guid,
                        Body = c.Body,
                        Created = DateTime.SpecifyKind(c.Created, DateTimeKind.Utc),
                        CreatedBy = c.CreatedBy,
                        CreatedByUsername = creator.Username,
                        CreatedByFacebookUserId = creator.FacebookUserId,
                        Role = order.CreatedByAccountGuid == c.CreatedBy ? "Seller" : String.Empty
                    };
                });

                return converted;
            }
        }

   
    }
}