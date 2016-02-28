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
    [BasicAuthentication]
    public class TradeOfferController : ApiController
    {
        [AcceptVerbs("GET")]
        [Route("~/v1/tradeOffer/{tradeOfferGuid}")]
        public TradeOfferView Get(Guid tradeOfferGuid, string lang = "en-US")
        {
            //you can only view if you are the to party
            Account account = AccountController.GetAccountByUsername(User.Identity.Name);

            using (DatabaseContext context = new DatabaseContext())
            {
                var offer = (from o in context.TradeOffers
                             where o.Guid == tradeOfferGuid
                             select o).FirstOrDefault();

                if (offer == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (offer.ToAccountGuid != account.Guid
                    && offer.FromAccountGuid != account.Guid)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                OrderView orderView = new OrderViewController().Get(offer.OrderGuid, lang);
                SkuView relatedSku = new SkuViewController().Get(orderView.SkuGuid, lang);
                Account from = (from a in context.Accounts
                                where a.Guid == offer.FromAccountGuid
                                select a).First();

                Account to = (from a in context.Accounts
                              where a.Guid == offer.ToAccountGuid
                              select a).First();

                TradeOfferView converted = new TradeOfferView
                {
                    Guid = offer.Guid,
                    Created = offer.Created,
                    Currency = offer.Currency,
                    FromAccountGuid = offer.FromAccountGuid,
                    LastModified = offer.LastModified,
                    OrderGuid = offer.OrderGuid,
                    Price = offer.Price,
                    ProductColourGuid = orderView.ProductColourGuid,
                    ProductColourName = orderView.ProductColourName,
                    ProductCondition = orderView.ProductCondition,
                    ProductConditionName = orderView.ProductConditionName,
                    Location = orderView.Location,
                    LocationName = orderView.LocationName,
                    Quantity = offer.Quantity,
                    SkuGuid = orderView.SkuGuid,
                    SkuImageUrl = orderView.ImageSrc,
                    SkuFullName = orderView.SkuFullName,
                    Total = offer.Quantity * offer.Price,
                    TradeCode = offer.TradeCode,
                    Type = orderView.Type,
                    FromUserName = from.Username,
                    ToAccountGuid = offer.ToAccountGuid,
                    ToUserName = to.Username,
                    CancelReason = offer.CancelReason,
                    Status = GetStatus(offer),
                    Deadline = offer.Deadline
                };

                return converted;
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/tradeOffer/{tradeOfferGuid}/accept")]
        public void AcceptOffer(Guid tradeOfferGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).FirstOrDefault();

                if (offer == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (!IsAuthorized(offer))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                if (!IsPending(offer))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                ProcessOrder(offer);

                offer.LastModified = DateTime.UtcNow;
                offer.IsAccepted = true;
                context.SaveChanges();

                TradeOfferActionNotifier.RunPort.Post(new EmptyValue());
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/tradeOffer/{tradeOfferGuid}/cancel")]
        public void CancelOffer(Guid tradeOfferGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).FirstOrDefault();

                if (offer == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (!IsAuthorized(offer))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                if (!IsPending(offer))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                offer.LastModified = DateTime.UtcNow;
                offer.IsCancelled = true;
                context.SaveChanges();

                TradeOfferActionNotifier.RunPort.Post(new EmptyValue());
            }
        }

        bool IsPending(TradeOffer offer)
        {
            return GetStatus(offer) == "Pending";
        }

        bool IsAuthorized(TradeOffer offer)
        {
            Account account = AccountController.GetAccountByUsername(User.Identity.Name);
            if (offer.ToAccountGuid != account.Guid)
            {
                return false;
            }

            return true;
        }

        private void ProcessSellOrder(TradeOffer offer)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Order order = (from o in context.Orders
                               where o.Guid == offer.OrderGuid
                               select o).FirstOrDefault();

                Account seller = (from a in context.Accounts
                                  where a.Guid == order.CreatedByAccountGuid
                                  select a).FirstOrDefault();

                if (offer.Quantity < order.MinimumOrder) //minimum not met
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                //offer is made buyer to seller
                var buyer = (from a in context.Accounts
                             where a.Guid == offer.FromAccountGuid
                             select a).First();

                Trade trade = new Trade
                {
                    Guid = Guid.NewGuid(),
                    TradeCode = offer.TradeCode,
                    SellerGuid = order.CreatedByAccountGuid,
                    SellerUserName = seller.Username,
                    SellerPhoneNumber = seller.Phone,
                    SellerLanguageCode = seller.LanguageCode,
                    BuyerGuid = buyer.Guid,
                    BuyerUserName = buyer.Username,
                    BuyerPhoneNumber = buyer.Phone,
                    BuyerLanguageCode = buyer.LanguageCode,
                    Created = DateTime.UtcNow,
                    Currency = offer.Currency,
                    OrderGuid = order.Guid,
                    Price = offer.Price,
                    Quantity = offer.Quantity,
                    Total = offer.Price * offer.Quantity,
                    LastModified = DateTime.UtcNow,
                    Status = TradeStatus.InProgress.ToString(),
                    SkuGuid = order.SkuGuid
                };

                context.Trades.Add(trade);
                order.Quantity = order.Quantity - offer.Quantity;
                context.SaveChanges();
            }
        }

        private Trade ProcessBuyOrder(TradeOffer offer)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //offer is made from seller to buyer
                Account seller = (from a in context.Accounts
                                  where a.Guid == offer.FromAccountGuid
                                  select a).FirstOrDefault();

                Account buyer = (from a in context.Accounts
                                 where a.Guid == offer.ToAccountGuid
                                 select a).FirstOrDefault();

                Order order = (from o in context.Orders
                               where o.Guid == offer.OrderGuid
                               select o).FirstOrDefault();

                //okay user is verified. lets make the trade happen
                Trade trade = new Trade
                {
                    Guid = Guid.NewGuid(),
                    TradeCode = offer.TradeCode,
                    SellerGuid = seller.Guid,
                    SellerUserName = seller.Username,
                    SellerLanguageCode = seller.LanguageCode,
                    SellerPhoneNumber = seller.Phone,
                    BuyerGuid = buyer.Guid,
                    BuyerUserName = buyer.Username,
                    BuyerPhoneNumber = buyer.Phone,
                    BuyerLanguageCode = buyer.LanguageCode,
                    Created = DateTime.UtcNow,
                    Currency = offer.Currency,
                    OrderGuid = order.Guid,
                    Price = offer.Price,
                    Quantity = offer.Quantity,
                    Total = offer.Price * offer.Quantity,
                    LastModified = DateTime.UtcNow,
                    Status = TradeStatus.InProgress.ToString(),
                    SkuGuid = order.SkuGuid
                };

                context.Trades.Add(trade);
                order.Quantity = order.Quantity - offer.Quantity;
                context.SaveChanges();
                return trade;
            }
        }

        private void ProcessOrder(TradeOffer tradeOffer)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                Order order = (from o in context.Orders
                               where o.Guid == tradeOffer.OrderGuid
                               select o).First();

                if (order.Type == "Sell")
                {
                    ProcessSellOrder(tradeOffer);
                }
                else
                {
                    ProcessBuyOrder(tradeOffer);
                }
            }
        }

        static string GetStatus(TradeOffer offer)
        {
            return offer.IsAccepted ? "Accepted" : offer.IsCancelled ? "Cancelled" : DateTime.UtcNow > offer.Deadline ? "Expired" : "Pending";
        }
    }
}