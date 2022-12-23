using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSN.Resa.Core.Commons.Extention
{
    public static class ExceptionExtention
    {
        public static string GetFullMessage(this Exception exception)
        {
            var exceptionMessageBuilder = new StringBuilder(exception.Message);

            int itterationCount = 0;

            Exception ex = exception;

            while ((ex = ex.InnerException) != null)
            {
                itterationCount++;
                var indent = string.Concat(Enumerable.Repeat("\t", itterationCount));
                exceptionMessageBuilder.Append(System.Environment.NewLine);
                exceptionMessageBuilder.Append(indent);
                exceptionMessageBuilder.Append("-------------------------INNER_EXCEPTION-------------------");
                exceptionMessageBuilder.Append(System.Environment.NewLine);
                exceptionMessageBuilder.Append(indent);
                exceptionMessageBuilder.Append(ex.Message);
            }

            return exceptionMessageBuilder.ToString();
        }
    }
}
