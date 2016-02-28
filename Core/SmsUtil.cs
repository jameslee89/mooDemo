using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class SmsUtil
    {
        public static void SendMessage(string number, string message, string senderId)
        {
            CMTelecomSmsUtil.SendMessage(number, message, senderId);
        }
    }
}
