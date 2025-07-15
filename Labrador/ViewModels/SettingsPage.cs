
using Labrador.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class SettingsPage : ContentPage
    {
        Entry pinEntry;

        public async Task AskForParent()
        {
            await TextToSpeech.SpeakAsync("This page requires an adults help. Please ask your carer for help");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = AskForParent();
        }

        protected override void OnDisappearing()
        {
            Content = GetLoginPage();
            (Application.Current as App).configuration.ReadConfiguration(); // This ensures that any unsaved changes do not follow over
            base.OnDisappearing();
        }

        protected StackLayout GetLoginPage()
        {
            StackLayout login = new StackLayout();

            Button loginButton = new Button { 
                Text = "Login", 
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            pinEntry = new Entry
            {
                Placeholder = "####",
                PlaceholderColor = Color.LightGray,
                HorizontalOptions = LayoutOptions.Center,
                Keyboard = Keyboard.Telephone
            };

            loginButton.Clicked += LoginButton_Clicked;

            login.Children.Add(new Label
            {
                Text = "Configuration Page",
                FontSize = 40,
                HorizontalTextAlignment = TextAlignment.Center
            });
            login.Children.Add(new Label
            {
                Text = "Please Ask for An Adults Help",
                HorizontalTextAlignment = TextAlignment.Center
            });

            login.Children.Add(new Label
            {
                Text = "Please enter your PIN Number Below",
                HorizontalTextAlignment = TextAlignment.Center
            });

            login.Children.Add(pinEntry);
            login.Children.Add(loginButton);

            return login;
        }

        

        private async void LoginButton_Clicked(object sender, System.EventArgs e)
        {
            int pinNumber;
            if(Int32.TryParse(pinEntry.Text, out pinNumber))
            {
                if(pinNumber == (Application.Current as App).configuration.GetPIN())
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ConfigurationPage());
                }
            }
        }

        public SettingsPage()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationSettingsLabel;
            IconImageSource = (Application.Current as App).imageLoader.ResourceImage("cog.png").Source;

            Content = GetLoginPage();
        }
    }
}