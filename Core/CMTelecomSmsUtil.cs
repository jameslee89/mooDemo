using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkenLabs.Market.Core
{
    class CMTelecomSmsUtil
    {
        public static void SendMessage(string number, string message, string senderId)
        {
            //trim spaces
            string formattedNumber = number.Replace(" ", "");
            const string url = "https://sgw01.cm.nl/gateway.ashx";
            Guid productToken = new Guid("1d79995f-ec8f-4b98-9b58-f33dcbc1d54b");
            var xml = CreateSmsXml(productToken, senderId, formattedNumber, message);
            var response = DoHttpPost(url, xml);
        }

        private static string DoHttpPost(string url, string requestString)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                return webClient.UploadString(url, "POST", requestString);
            }
        }

        private static string CreateSmsXml(Guid productToken, string sender, string recipient, string message)
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("MESSAGES",
                            new XElement("AUTHENTICATION",
                                new XElement("PRODUCTTOKEN", productToken)
                            ),
                            new XElement("MSG",
                                new XElement("FROM", sender),
                                new XElement("TO", recipient),
                                new XElement("DCS", 8),
                                new XElement("MINIMUMNUMBEROFMESSAGEPARTS", 1),
                                new XElement("MAXIMUMNUMBEROFMESSAGEPARTS", 8),
                                new XElement("BODY", message)
                            ))).ToString(SaveOptions.None);
        }

    }
}
