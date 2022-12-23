using BSN.Resa.DoctorApp.Domain.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;

namespace BSN.Resa.DoctorApp.Views.Utilities
{
    public static class CollectionsExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }

        public static List<CallbackRequestBindableObject> ToCallbackRequestWrappers(this IEnumerable<CallbackRequest> callbackRequests)
        {
            return ToCallbackRequestWrappers(callbackRequests?.ToList());
        }

        public static List<CallbackRequestBindableObject> ToCallbackRequestWrappers(this IList<CallbackRequest> callbackRequests)
        {
            if (callbackRequests == null || !callbackRequests.Any())
                return new List<CallbackRequestBindableObject>();

            var callbackRequestWrappers = new List<CallbackRequestBindableObject>();

            foreach (var theCall in callbackRequests)
            {
                callbackRequestWrappers.Add(new CallbackRequestBindableObject(theCall));
            }

            return callbackRequestWrappers;
        }

        public static List<CallbackRequest> ToCallbackRequests(this IList<CallbackRequestBindableObject> callbackRequestsViewModels)
        {

            if (callbackRequestsViewModels == null || !callbackRequestsViewModels.Any())
                return new List<CallbackRequest>();

            var callbackRequests = new List<CallbackRequest>();

            foreach (var theCall in callbackRequestsViewModels)
            {
                if (theCall.CallbackRequest != null)
                    callbackRequests.Add(theCall.CallbackRequest);
            }

            return callbackRequests;
        }
    }
}
