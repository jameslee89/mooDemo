using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    class MyCoolSmsUtil
    {
        public static void SendMessage(string number, string message, string senderId)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["username"] = "linkenlabs";
                data["password"] = "2I{fxQ2;?^D7i%M";
                data["function"] = "sendSms";
                data["number"] = number;
                data["message"] = message;
                data["senderid"] = senderId;

                var response = wb.UploadValues(@"https://www.my-cool-sms.com/api-socket.php", "POST", data);

                string jsonResponse = System.Text.Encoding.ASCII.GetString(response);

                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                if (values["success"] != "True")
                {
                    throw new Exception(values["description"]);
                }
            }
        }
    }
}
