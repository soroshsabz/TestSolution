using System;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class CallbackRequestsListViewCell : ViewCell
    {
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(View != null)
                View.SizeChanged += ViewOnSizeChanged;

            if (_callbackRequestBindableObject != null)
            {
                _callbackRequestBindableObject.OnCallbackRequestDeleteRequested -= DeleteCallbackRequest;
            }

            _callbackRequestBindableObject = BindingContext as CallbackRequestBindableObject;

            if (_callbackRequestBindableObject != null)
            {
                _callbackRequestBindableObject.OnCallbackRequestDeleteRequested += DeleteCallbackRequest;

            }
        }

        private void ViewOnSizeChanged(object sender, EventArgs e)
        {
            _viewCellWidth = View.Width;
        }

        public async Task DeleteCallbackRequest()
        {
            await View.TranslateTo(_viewCellWidth * -1.0 , 0, 1000);
            await View.TranslateTo(0, 0, 0);
            View.SizeChanged -= ViewOnSizeChanged;
        }

        private double _viewCellWidth;
        private CallbackRequestBindableObject _callbackRequestBindableObject;
    }
}