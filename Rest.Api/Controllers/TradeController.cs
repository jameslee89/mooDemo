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
    public class TradeController : ApiController
    {

        [BasicAuthentication]
        public TradeOffer Post(TradeRequest request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Order order = (from o in context.Orders
                               where o.Guid == request.OrderGuid
                               select o).First();

                Account account = AccountController.GetAccountByUsername(User.Identity.Name);

                TradeOffer offer = new TradeOffer
                {
                    Guid = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    TradeCode = Util.GenerateTicketNumber(6),
                    Currency = order.Currency,
                    OrderGuid = request.OrderGuid,
                    Quantity = request.Quantity,
                    Price = order.Price,
                    ToAccountGuid = order.CreatedByAccountGuid,
                    Deadline = DateTime.UtcNow.AddDays(1),
                    FromAccountGuid = account.Guid
                };

                context.TradeOffers.Add(offer);
                context.SaveChanges();

                TradeOfferCreatedNotifier.RunPort.Post(new EmptyValue());
                return offer;
            }
        }


    }
}