using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace EatZD
{
    class MatchLatestTime
    {
        private static Dictionary<string,int> htTime = new Dictionary<string, int>();
        public static int GetLatestTime(string key)
        {
            int ret = 0;
            if(htTime.ContainsKey(key))
            {
                ret = htTime[key];
            }
            return ret;
        }
        public static void SetLatestTime(string key, int value)
        {
            if(htTime.ContainsKey(key))
            {
                htTime[key] = value;
            }
            else
            {
                htTime.Add(key,value);
            }
        }
    }
}
