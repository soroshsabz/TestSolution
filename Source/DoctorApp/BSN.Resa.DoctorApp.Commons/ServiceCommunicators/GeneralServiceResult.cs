using System.Net;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
    public class GeneralServiceResult<T> : IServiceResult<T>
    {
        public T Data { get; set; }
        public ServiceResultCode ResultCode { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
