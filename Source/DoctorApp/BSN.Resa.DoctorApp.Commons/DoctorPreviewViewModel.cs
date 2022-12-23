using BSN.Resa.Core.Commons;
using BSN.Resa.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.DoctorApp.Commons
{
    public class DoctorPreviewViewModel
    {
        [Display(Name = "FirstName", ResourceType = typeof(Resources))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resources))]
        public string LastName { get; set; }

        public string MSISDN { get; set; }

        [Display(Name = "VSIN", ResourceType = typeof(Resources))]
        public string VSIN => MSISDN?.Right(CardConstants.VsinLength);

        [Display(Name = "State", ResourceType = typeof(Resources))]
        public DoctorState State { get; set; }
    }
}
