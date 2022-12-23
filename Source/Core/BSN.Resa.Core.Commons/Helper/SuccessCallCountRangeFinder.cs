using System;
using System.Linq;
namespace BSN.Resa.Core.Commons.Helper
{
    public static class SuccessCallCountRangeFinder
    {
        public static String Find(int count)
        {
            if (count == 0)
                return "+30";
            else if (Enumerable.Range(1, 100).Contains(count))
                return "+50";
            else if (Enumerable.Range(101, 200).Contains(count))
                return "+100";
            else if (Enumerable.Range(201, 500).Contains(count))
                return "+200";
            else if (Enumerable.Range(501, 1000).Contains(count))
                return "+500";
            else if (Enumerable.Range(1001, 2000).Contains(count))
                return "+1000";
            else if (Enumerable.Range(2001, 5000).Contains(count))
                return "+2000";
            else if (Enumerable.Range(5001, 10000).Contains(count))
                return "+5000";
            else if (Enumerable.Range(10001, 20000).Contains(count))
                return "+10000";
            else if (Enumerable.Range(20001, 50000).Contains(count))
                return "+20000";
            else if (Enumerable.Range(50001, 1000000).Contains(count))
                return "+50000";
            return "+1000000";
        }
    }
}