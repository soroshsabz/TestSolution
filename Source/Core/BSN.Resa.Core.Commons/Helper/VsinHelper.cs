using System;

namespace BSN.Resa.Core.Commons
{
    public class VsinHelper
    {
        public static long VsinToMsisdn(long vsin)
        {
            var len = vsin.ToString().Length;
            if (len > 10)
            {
                throw new Exception("invalid vsin");
            }
            double middlePart = 1111111111 % Math.Pow(10, 10 - len) * Math.Pow(10, len);
            return 98330000000000 + (long)middlePart + vsin;
        }
    }
}