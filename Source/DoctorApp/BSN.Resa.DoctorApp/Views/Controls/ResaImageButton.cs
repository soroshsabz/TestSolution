using System.Windows.Input;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class ResaImageButton : Image
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
            typeof(ICommand), typeof(ResaImageButton));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter), typeof(object), typeof(ResaImageButton));

        public static readonly BindableProperty IconProperty = BindableProperty.Create(
            nameof(Icon), typeof(string), typeof(ResaImageButton), default(string));

        public ResaImageButton()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            BackgroundColor = Color.Transparent;
            Aspect = Aspect.AspectFit;

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Opacity = 0.3;
                    BackgroundColor = (Color)Application.Current.Resources["AppPrimaryColor"];
                    this.FadeTo(1);
                    BackgroundColor = Color.Transparent;
                    Icon_OnClicked();
                })
            });

            SetBinding(SourceProperty, new Binding(nameof(Icon), source:this));
        }

        public virtual string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
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

        protected virtual void Icon_OnClicked()
        {
            if (Command == null || !Command.CanExecute(null))
                return;
            Command.Execute(CommandParameter);
        }
    }
}