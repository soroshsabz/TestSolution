using System.Net;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
    /// <summary>
    /// This will be a streamlined wrapper for result of all web APIs requests.
    /// The idea is to return an object in which both Data(in case of success) and result code exist,
    /// so that we can act accordingly depending on result code. This idea is similar to
    /// HttpClient itself(IsSuccessStatusCode, HttpStatusCode, ...).
    /// Further reading:
    /// https://softwareengineering.stackexchange.com/questions/159096/return-magic-value-throw-exception-or-return-false-on-failure
    /// </summary>
    public interface IServiceResult<T>
    {
        //The actual data returning from service such as a deserialized json returning from web API
        T Data { get; set; }

        //this is the first thing needed to be checked. For example before
        //using Data this SHOULD be checked and be "Success".
        ServiceResultCode ResultCode { get; set; }

        /// <summary>
        /// This is for further investigating web API's request response.
        /// Usually Checking ResultCode would be enough.
        /// </summary>
        HttpStatusCode HttpStatusCode { get; set; }

        string ErrorMessage { get; set; }
    }

    public enum ServiceResultCode
    {
        Undefined = 0,
        Success = 100, // everything went well. WARNING: only in this condition we must use Data object of IServiceResult
        JsonException = 200, //Some possible cases are changes in server's json model. NOTE: HttpStatusCode of IServiceResult in this case is "OK".
//        HttpStatusNotOk = 3, //whenever HTTP's response is 200 OK. Only if this is false, HttpStatusCode property of IServiceResult is set and can be checked.
        NetworkProblem = 400,
        NoConnectivity = 500, //Cases such as no Internet is available.
        AddressNotReachable = 600,
        AuthenticationFailed = 700,//Cases such wrong username/password, expired or wrong token. In this case HttpStatusCode could be Forbidden, Unauthorized or BadRequest.
        AppInternalProblem = 1009, //This is a general-purpose Error when not other more specific errors have been set.
    }
}
