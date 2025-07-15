using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class DailyStories : ContentPage
    {
        public DailyStories()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationDailyStoriesLabel;
            IconImageSource = (Application.Current as App).imageLoader.ResourceImage("book.png").Source;
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Daily Stories Page" }
                }
            };
        }
    }
}