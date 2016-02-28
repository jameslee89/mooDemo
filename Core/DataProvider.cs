using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class DataProvider
    {
        public static string FormatRelativeTimeAgo(DateTime time)
        {
            TimeSpan timespan = DateTime.UtcNow.Subtract(time);

            List<string> list = new List<string>();

            if (timespan.Days > 0)
            {
                list.Add(String.Format("{0}d", timespan.Days));
            }
            else if (timespan.Hours > 0)
            {
                list.Add(String.Format("{0}h", timespan.Hours));
            }
            else
            {
                list.Add("<1h");
            }

            return String.Join(" ", list.ToArray());


        }
    }
}
