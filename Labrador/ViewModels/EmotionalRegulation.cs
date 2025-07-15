
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class EmotionalRegulation : ContentPage
    {
        public EmotionalRegulation()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationEmotionalRegLabel;
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "PAIN INDICATOR NOT YET IMPLEMENTED" }
                }
            };
        }
    }
}