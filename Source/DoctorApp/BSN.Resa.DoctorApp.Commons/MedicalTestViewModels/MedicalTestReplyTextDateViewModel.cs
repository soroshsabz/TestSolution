using Newtonsoft.Json;

namespace BSN.Resa.DoctorApp.Commons.MedicalTestViewModels
{
    public class MedicalTestReplyTextDateViewModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}