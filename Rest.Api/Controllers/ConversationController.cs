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
    public class ConversationController : ApiController
    {
        internal bool IsAuthorised(Guid accountGuid)
        {
            Account userAccount = AccountController.GetAccountByUsername(User.Identity.Name);
            return userAccount.Guid == accountGuid;
        }

        //[BasicAuthentication]
        //[Route("~/v1/conversation")]
        //public IEnumerable<ConversationInfo> GetConversations()
        //{
        //    Account user = new AccountController().GetAccountByUsername(User.Identity.Name);

        //    //get distinct last message by sender 
        //    using (DatabaseContext context = Util.CreateContext())
        //    {
        //        //incoming
        //        Guid userAccountGuid = new AccountController().GetAccountByUsername(User.Identity.Name).Guid;

        //        var messages = (from m in context.Comments
        //                        where m.To == user.Guid || m.From == user.Guid
        //                        select new
        //                        {
        //                            OtherParty = m.To == user.Guid ? m.From : m.To,
        //                            Message = m
        //                        }).ToArray();

        //        var summary = (from m in messages
        //                       group m by m.OtherParty into g
        //                       select new
        //                       {
        //                           OtherParty = g.Key,
        //                           Messages = g.OrderByDescending(m => m.Message.Created).Select(m => m.Message).ToArray()
        //                       }).ToList();



        //        List<ConversationInfo> list = summary.ConvertAll(item =>
        //        {
        //            Account otherPartyAccount = (from a in context.Accounts
        //                                         where a.Guid == item.OtherParty
        //                                         select a).FirstOrDefault(); 
        //            int unreadCount = GetUnreadCount(userAccountGuid, item.Messages);
        //            var firstMessage = item.Messages.First();

        //            return new ConversationInfo
        //            {
        //                Created = firstMessage.Created,
        //                UnreadCount = unreadCount,
        //                LastMessage = firstMessage.Body,
        //                With = item.OtherParty,
        //                WithName = otherPartyAccount.Username,
        //                IsReceived = firstMessage.From == item.OtherParty
        //            };
        //        });

        //        return list.OrderByDescending(c => c.Created);
        //    }
        //}

        //int GetUnreadCount(Guid userGuid, Comment[] messages)
        //{
        //    int count = 0;
        //    if (messages.Length == 0)
        //    {
        //        return count;
        //    }
        //    foreach (var msg in messages)
        //    {
        //        if (msg.From == userGuid || msg.IsRead)
        //        {
        //            break;
        //        }
        //        count++;
        //    }
        //    return count;
        //}

        //// GET api/<controller>/5
        //[BasicAuthentication]
        //[Route("~/v1/conversation/{otherPartyGuid}")]
        //public IEnumerable<ConversationMessage> GetDialogues(Guid otherPartyGuid)
        //{
        //    Account userAccount = new AccountController().GetAccountByUsername(User.Identity.Name);

        //    //find all conversations between accountGuid and userAccount;
        //    using (DatabaseContext context = Util.CreateContext())
        //    {
        //        Account otherPartyAccount = (from a in context.Accounts
        //                                     where a.Guid == otherPartyGuid
        //                                     select a).First();

        //        var results = (from m in context.Comments
        //                       where (m.To == userAccount.Guid && m.From == otherPartyGuid)
        //                       || (m.To == otherPartyGuid && m.From == userAccount.Guid)
        //                       orderby m.Created ascending
        //                       select m).ToList();

        //        var converted = results.ConvertAll(m =>
        //        {
        //            return new ConversationMessage
        //            {
        //                From = m.From == otherPartyGuid ? otherPartyAccount.Username : userAccount.Username,
        //                Body = m.Body,
        //                Created = m.Created,
        //                OrderGuid = m.OrderGuid,
        //            };
        //        });
        //        return converted;
        //    }
        //}

        //[BasicAuthentication]
        //[Route("~/v1/conversation/{otherPartyGuid}/read")]
        //public void Post(Guid otherPartyGuid)
        //{
        //    Account account = new AccountController().GetAccountByUsername(User.Identity.Name);

        //    using (DatabaseContext context = Util.CreateContext())
        //    {
        //        var messages = (from m in context.Comments
        //                        where (m.To == account.Guid && m.From == otherPartyGuid)
        //                        || (m.To == otherPartyGuid && m.From == account.Guid)
        //                        where !m.IsRead
        //                        select m).ToList();
        //        foreach (var message in messages)
        //        {
        //            message.IsRead = true;
        //        }
        //        context.SaveChanges();
        //    }
        //}


    }
}