using System.IO;
using System.Web;

namespace BSN.Resa.Commons
{
	public static class HttpRequestExtension
    {
        public static string ReceivedData(this HttpRequestBase request)
        {
            request.InputStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(request.InputStream).ReadToEnd();
        }
    }
}