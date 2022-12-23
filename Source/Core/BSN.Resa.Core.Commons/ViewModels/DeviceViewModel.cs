using BSN.Resa.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class DeviceViewModel
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        [Display(Name = "OperatingSystem", ResourceType = typeof(Resources))]
        public DeviceOperatingSystem OperatingSystem { get; set; }

        [Display(Name = "DeviceModel", ResourceType = typeof(Resources))]
        public string DeviceModel { get; set; }

        [Display(Name = "IsPrimary", ResourceType = typeof(Resources))]
        public bool IsPrimary { get; set; }

        [Display(Name = "AccountId", ResourceType = typeof(Resources))]
        public string AccountId { get; set; }
    }
}
