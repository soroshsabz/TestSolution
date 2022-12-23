using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class CustomListView : ListView
    {
        public CustomListView() : base (ListViewCachingStrategy.RecycleElement)
        {
        }
    }
}