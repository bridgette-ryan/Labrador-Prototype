using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Labrador.Models;
using System.Text.RegularExpressions;

namespace Labrador.ViewModels
{
    public class InitializePageCS : ContentPage
    {
        Picker languagePicker;
        Entry pinEntry;
        Configuration config;
        public async Task AskForParent()
        {
            await TextToSpeech.SpeakAsync("Welcome to Labrador. If you are a child, please ask for an adult's help");
        }

        public InitializePageCS()
        {
            config = (Application.Current as App).configuration;
            languagePicker = new Picker { Title = "Language" };
            foreach (Languages language in config.GetLanguages())
            {
                languagePicker.Items.Add(language.Language);
            }

            pinEntry = new Entry { Placeholder = "####", PlaceholderColor = Color.LightGray, HorizontalOptions = LayoutOptions.Center, Keyboard=Keyboard.Telephone };
            var confirmButton = new Button { Text = "Confirm", HorizontalOptions = LayoutOptions.Center  };
            confirmButton.Clicked += ConfirmButton_Clicked;

            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Welcome to Labrador.", FontSize = 20, HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = "This software is a Speech Generation Device for children on the ASD spectrum. As this is the first time you have run this software, you will need to configure a few settings.", FontSize = 14, HorizontalTextAlignment = TextAlignment.Start, LineBreakMode = LineBreakMode.WordWrap },
                    languagePicker,
                    new Label { Text = "PIN", FontSize = 14, HorizontalTextAlignment = TextAlignment.Center},
                    pinEntry,
                    confirmButton
                }
            };
            _ = AskForParent();
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            bool save = true;

            if (languagePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please Select A Language", "Ok");
                save = false;
            }
            else
            {
                config.SetLanguage(languagePicker.SelectedItem.ToString());
            }

            Regex rx = new Regex(@"^[0-9][0-9][0-9][0-9]$");


            if(string.IsNullOrEmpty(pinEntry.Text)) {
                await DisplayAlert("Error", "Please enter a 4 digit PIN", "Ok");
                save = false;
            }
            else if (!rx.IsMatch(pinEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a 4 digit PIN", "Ok");
                save = false;
            }
            if (save)
            {
                config.WriteConfiguration();
                config.WritePIN(Int32.Parse(pinEntry.Text));
                await DisplayAlert("Success", "Please make sure you remember your PIN.", "Ok");
                await Navigation.PushModalAsync(new NavigationPane());
            }
        }
    }
}