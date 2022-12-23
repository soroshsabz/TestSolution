using System.Windows.Input;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class ClickableContentView : ContentView
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
            typeof(ICommand), typeof(ResaImageButton));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter), typeof(object), typeof(ResaImageButton));

        public ClickableContentView()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Opacity = 0.3;
                    BackgroundColor = (Color)Application.Current.Resources["AppPrimaryColor"];
                    this.FadeTo(1);
                    BackgroundColor = Color.Transparent;
                    OnClicked();
                })
            });
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private void OnClicked()
        {
            if (Command == null || !Command.CanExecute(null))
                return;
            Command.Execute(CommandParameter);
        }
    }
}
