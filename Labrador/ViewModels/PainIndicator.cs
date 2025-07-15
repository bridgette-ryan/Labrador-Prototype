
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class PainIndicator : ContentPage
    {
        public PainIndicator()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationPainIndicatorLabel;
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "PAIN INDICATOR NOT YET IMPLEMENTED" }
                }
            };
        }
    }
}