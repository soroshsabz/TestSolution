using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Utilities
{
	public class FocusTriggerAction : TriggerAction<Entry>
	{
		public bool Focused { get; set; }

		protected override void Invoke(Entry searchEntry)
		{
			if (Focused)
				searchEntry.Focus();
			else
				searchEntry.Unfocus();
		}
	}
}