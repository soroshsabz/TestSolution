using System.ComponentModel.DataAnnotations;
using BSN.Resa.Locale;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class PricingPolicyUpdate
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFieldValidationMessage", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "Title", ResourceType = typeof(Resources))]
        public string Title { get; set; }
    }
}
