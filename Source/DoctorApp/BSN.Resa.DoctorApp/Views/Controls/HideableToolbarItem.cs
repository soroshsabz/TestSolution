using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
	/// <summary>
	/// Visit https://forums.xamarin.com/discussion/40622/found-a-way-to-make-toolbaritems-visible-invisible-without-a-custom-renderer
	/// </summary>
	public class HideableToolbarItem : ToolbarItem
	{
		public HideableToolbarItem()
		{
			InitVisibility();
		}

		private void InitVisibility()
		{
			OnIsVisibleChanged(this, false, IsVisible);
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();
			InitVisibility();
		}

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static BindableProperty IsVisibleProperty =
			BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(HideableToolbarItem), false,
				propertyChanged: (bindableObject, oldValue, newValue) => { OnIsVisibleChanged(bindableObject, (bool)oldValue, (bool)newValue); });



		private static void OnIsVisibleChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			var item = bindable as HideableToolbarItem;

			if (item?.Parent == null)
				return;

			var items = ((ContentPage)item.Parent).ToolbarItems;

			if (newvalue && !items.Contains(item))
				Device.BeginInvokeOnMainThread(() => items.Add(item));
			else if (!newvalue && items.Contains(item))
				Device.BeginInvokeOnMainThread(() => items.Remove(item));
		}
	}
}
