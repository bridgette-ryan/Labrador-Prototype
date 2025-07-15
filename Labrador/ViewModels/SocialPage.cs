
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class SocialPage : ContentPage
    {
        public SocialPage()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationSocialIntLabel;
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "SOCIALIZATION NOT YET IMPLEMENTED" }
                }
            };
        }
    }
}