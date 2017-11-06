using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    // 흠 일단 임시
    public class CompareName
    {
        public static bool Match(string fullname, string subscription)
        {
            subscription = subscription.Replace(" ", "").ToLower();
            fullname = fullname.Replace(" ", "").ToLower();

            return fullname.Contains(subscription);
        }
    }
}
