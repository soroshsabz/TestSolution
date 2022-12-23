using BSN.Resa.DoctorApp.Commons.Utilities;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.Utilities
{
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		public string Text { get; set; }

        public string StringFormat { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(Text))
				return null;

            string localeText = Locale.Resources.ResourceManager.GetString(Text);

            if(!StringFormat.IsNullOrEmptyOrSpace())
                return string.Format(StringFormat, localeText);

            return localeText;
        }
	}
}
