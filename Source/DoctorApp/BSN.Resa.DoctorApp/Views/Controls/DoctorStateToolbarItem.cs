using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class DoctorStateToolbarItem : ResaToolbarItem
    {
        public DoctorStateToolbarItem()
        {
            SetBinding(IconProperty, new Binding("DoctorStateImageUrl"));
            SetBinding(IsEnabledProperty, new Binding("CanChangeDoctorState"));
            SetBinding(CommandProperty, new Binding("ChangeDoctorStateCommand"));
        }
    }
}
