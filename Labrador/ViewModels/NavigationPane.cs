using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class NavigationPane : TabbedPage
    {
        public NavigationPane()
        {
            BarBackgroundColor = Color.White;
            BarTextColor = Color.Black;
            NavigationPage.SetHasNavigationBar(this, false); // remove the title for the navigation pane, cause it takes up too much realestate

            //Check to see if current language selection is LtR or RtL
            if ((Application.Current as App).configuration.GetRTL())
            {
                //NavigationPane setup for Right To Left Languages (i.e. Arabic)
                this.Children.Add(new SettingsPage());
                if ((Application.Current as App).configuration.GetSocialInteractions()) this.Children.Add(new SocialPage());
                if ((Application.Current as App).configuration.GetEmotionalRegulation()) this.Children.Add(new EmotionalRegulation());
                if ((Application.Current as App).configuration.GetPainIndicator()) this.Children.Add(new PainIndicator());
                if ((Application.Current as App).configuration.GetDailyStories()) this.Children.Add(new DailyStories());
                if ((Application.Current as App).configuration.GetShowPicto()) this.Children.Add(new PictogramToSpeech());
            }
            else
            {
                //NavigationPane setup for Left To Right Languages (i.e. English)
                if ((Application.Current as App).configuration.GetShowPicto()) this.Children.Add(new PictogramToSpeech());
                if ((Application.Current as App).configuration.GetDailyStories()) this.Children.Add(new DailyStories());
                if ((Application.Current as App).configuration.GetPainIndicator()) this.Children.Add(new PainIndicator());
                if ((Application.Current as App).configuration.GetEmotionalRegulation()) this.Children.Add(new EmotionalRegulation());
                if ((Application.Current as App).configuration.GetSocialInteractions()) this.Children.Add(new SocialPage());
                this.Children.Add(new SettingsPage());
            }
        }
    }
}