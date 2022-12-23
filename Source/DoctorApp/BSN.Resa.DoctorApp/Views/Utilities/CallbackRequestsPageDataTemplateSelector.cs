using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Utilities
{
    public class CallbackRequestsPageDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var callbackRequestWrapper = item as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null)
                return WhenCallerHasNotNameTemplate;

            if (callbackRequest.ReturnCallHasBeenEstablished)
            {
                if (callbackRequest.CallerFullName.IsNullOrEmptyOrSpace())
                    return WhenCallbackRequestIsEstablishedAndHasNotNameTemplate;

                return WhenCallbackRequestIsEstablishedAndHasNameTemplate;
            }
            
            if (callbackRequest.CallerFullName.IsNullOrEmptyOrSpace())
                return WhenCallerHasNotNameTemplate;

            return WhenCallerHasNameTemplate;
        }

        public DataTemplate WhenCallerHasNameTemplate { get; set; }
        public DataTemplate WhenCallerHasNotNameTemplate { get; set; }
        public DataTemplate WhenCallbackRequestIsEstablishedAndHasNameTemplate { get; set; }
        public DataTemplate WhenCallbackRequestIsEstablishedAndHasNotNameTemplate { get; set; }
    }
}