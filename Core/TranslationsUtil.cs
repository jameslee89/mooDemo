using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class TranslationsUtil
    {
        private static Dictionary<string, string> en_US = new Dictionary<string, string>{
            { "HELLO", "Hello" },        
            { "BOOKINGCOMPLETEMESSAGE", "Your booking with mooketplace is confirmed and complete." },
            { "BOOKINGREFERENCE", "Booking Reference" },
            { "PICKUPINSTRUCTION", "Please present this message when you pick up." },
            { "ORDERDETAILS", "Order Details" },
            { "TOTAL", "Total" },
            { "NEWBUYORDER", "New Buy Order" },
            { "BUYER", "Buyer" },
            { "SELLER", "Seller" }
        };

        private static Dictionary<string, string> zh_TW = new Dictionary<string, string>{
            { "HELLO", "哈嘍" },       
            { "BOOKINGCOMPLETEMESSAGE", "预订与mooketplace确认" },
            { "BOOKINGREFERENCE", "預訂編號" },
            { "PICKUPINSTRUCTION", "請出示任何電子或紙質這封電子郵件的副本，當你拿起" },
            { "ORDERDETAILS", "訂單明細" },
            { "TOTAL", "總金額" },
            { "NEWBUYORDER", "新的採購訂單" },
            { "BUYER", "買主" },
            { "SELLER", "Seller" }
        };



        public static Dictionary<string, string> GetTranslationDictionary(string lang)
        {
            switch (lang)
            {
                case "en-US":
                    return en_US;
                case "zh-TW":
                    return zh_TW;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
