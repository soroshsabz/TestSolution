using Newtonsoft.Json;

namespace BSN.Resa.Commons.ViewModels
{
    public class ReCaptchaVerificationResponse
    {
        [JsonProperty("success")]
        public bool IsSuccessful { get; set; }

        [JsonProperty("challenge_ts")]
        public string ChallengeTimestamp { get; set; }

        [JsonProperty("hostname")]
        public string HostName { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
