using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Controllers;
using LinkenLabs.Market.RestApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeViewController : ApiController
    {
        [BasicAuthentication]
        public IEnumerable<TradeView> Get(string lang = "en-US")
        {
            Account account = AccountController.GetAccountByUsername(User.Identity.Name);

            using (DatabaseContext context = Util.CreateContext())
            {
                List<Trade> trades = (from t in context.Trades.AsNoTracking()
                                      where !t.IsDeleted
                                      && (t.BuyerGuid == account.Guid || t.SellerGuid == account.Guid)
                                      select t).ToList();

                List<SkuView> allSkus = new SkuViewController().GetList(lang: lang, showInactive: true).ToList();
                List<AccountInfo> allAccounts = new AccountController().GetAccounts().ToList();


                var result = trades.ConvertAll((trade) =>
                {
                    return Convert(trade, lang);
                });
                return result;
            }
        }

        public TradeView Convert(Trade trade, string lang)
        {
            SkuView relatedSku = new SkuViewController().Get(trade.SkuGuid, lang);
            OrderView orderView = new OrderViewController().Get(trade.OrderGuid, lang);

            return new TradeView
            {
                Guid = trade.Guid,
                TradeCode = trade.TradeCode,
                BuyerGuid = trade.BuyerGuid,
                BuyerUserName = trade.BuyerUserName,
                BuyerLanguageCode = trade.BuyerLanguageCode,
                BuyerPhoneNumber = trade.BuyerPhoneNumber,
                SellerGuid = trade.SellerGuid,
                SellerLanguageCode = trade.SellerLanguageCode,
                SellerUserName = trade.SellerUserName,
                SellerPhoneNumber = trade.SellerPhoneNumber,
                Created = trade.Created,
                Currency = trade.Currency,
                OrderGuid = trade.OrderGuid,
                ProductColourGuid = orderView.ProductColourGuid,
                ProductColourName = orderView.ProductColourName,
                ProductCondition = orderView.ProductCondition,
                ProductConditionName = orderView.ProductConditionName,
                Price = trade.Price,
                LastModified = trade.LastModified,
                Status = trade.Status,
                Quantity = trade.Quantity,
                SkuGuid = trade.SkuGuid,
                SkuFullName = relatedSku.FullName,
                Total = trade.Total
            };
        }

        [BasicAuthentication]
        public TradeView Get(Guid id, string lang = "en-US")
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var trade = (from t in context.Trades
                             where t.Guid == id
                             select t).FirstOrDefault();

                return Convert(trade, lang);
            }
        }

    }
}