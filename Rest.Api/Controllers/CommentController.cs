using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using LinkenLabs.Market.RestApi.Filters;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class CommentController : ApiController
    {
        [Route("~/v1/comment")]
        [BasicAuthentication]
        [HttpPost]
        public void Post(CommentPostRequest model)
        {
            if (String.IsNullOrEmpty(model.Body)
                || model.OrderGuid == Guid.Empty)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            using (DatabaseContext context = Util.CreateContext())
            {
                Account currentAccount = AccountController.GetAccountByUsername(User.Identity.Name);

                Order order = (from o in context.Orders.AsNoTracking()
                               where o.Guid == model.OrderGuid
                               select o).FirstOrDefault();

                if (order == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                Comment newComment = new Comment
                {
                    Guid = Guid.NewGuid(),
                    Body = model.Body,
                    Created = DateTime.UtcNow,
                    CreatedBy = currentAccount.Guid,
                    ReplyTo = model.ReplyTo,
                    OrderGuid = model.OrderGuid
                };

                context.Comments.Add(newComment);
                context.SaveChanges();

                OrderCommentNotifier.RunPort.Post(new EmptyValue());
            }
        }


    }
}