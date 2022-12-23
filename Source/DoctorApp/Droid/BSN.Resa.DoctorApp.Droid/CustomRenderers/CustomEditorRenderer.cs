using Android.Content;
using Android.Views;
using BSN.Resa.DoctorApp.Droid.CustomRenderers;
using BSN.Resa.DoctorApp.Views.Controls;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using TextAlignment = Android.Views.TextAlignment;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace BSN.Resa.DoctorApp.Droid.CustomRenderers
{
    /// <summary>
    /// Visit: https://stackoverflow.com/questions/48003093/xamarin-forms-hide-editor-or-entry-underline
    /// </summary>
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
                return;

            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);

            SetControlPadding();

            _defaultGravity = Control.Gravity;
            _defaultTextAlignment = Control.TextAlignment;

            AlignPlaceHolderCenter();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Editor.TextProperty.PropertyName)
            {
                if (Control.Text?.Length > 0)
                {
                    AlignPlaceHolderDefault();
                }
                else
                {
                    AlignPlaceHolderCenter();
                }
            }
        }

        private void SetControlPadding()
        {
            int padding = 10;
            Control.SetPadding(padding, padding, padding, padding);
        }

        private void AlignPlaceHolderCenter()
        {
            Control.TextAlignment = TextAlignment.Center;
            Control.Gravity = GravityFlags.Center;
        }

        private void AlignPlaceHolderDefault()
        {
            Control.TextAlignment = _defaultTextAlignment;
            Control.Gravity = _defaultGravity;
        }

        private TextAlignment _defaultTextAlignment;
        private GravityFlags _defaultGravity;
    }
}