using Newtonsoft.Json;

namespace BSN.Resa.DoctorApp.Commons.MedicalTestViewModels
{
    public class MedicalTestReplyTextViewModel
    {
        [JsonProperty("msg")]
        public MedicalTestReplyTextDateViewModel Msg { get; set; }
    }
}