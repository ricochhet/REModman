using REMod.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REModman.Utils;

namespace REMod.Helpers
{
    public class StatusNotifyHelper
    {
        public static string Assign(string String)
        {
            string temp = String ?? string.Empty;

            if (temp.Length > Constants.MAX_NOTIFY_CHAR_LENGTH)
            {
                temp = StringHelper.Truncate(temp, Constants.MAX_NOTIFY_CHAR_LENGTH, "...");
            }

            return temp;
        }
    }
}
