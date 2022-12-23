using System;
using System.ComponentModel.DataAnnotations;
using BSN.Resa.Core.Commons.Validators;
using BSN.Resa.Locale;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class OneTimeCharge
    {
        public int Id { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PhoneNumberRequired")]
        [MobileNumber(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PhoneNumberInvalid")]
        public string BeneficiaryPhoneNumber { get; set; }

        [Display(Name = "MSISDN", ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InputRequired")]
        public long BeneficiaryCardMSISDN { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InputRequired")]
        public int Amount { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SettledAt { get; set; }

        public bool IsSettled { get; set; }
    }
}
