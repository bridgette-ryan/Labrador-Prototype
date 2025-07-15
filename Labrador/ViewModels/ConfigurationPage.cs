using Labrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class ConfigurationPage : ContentPage
    {
        Grid settingsView, innerSGDPreview;
        Picker languagePicker;
        Image skinToneSample;
        Entry columns;
        CheckBox showSGD, showDS, showSI, showPI, showER;

        public ConfigurationPage()
        {
            NavigationPage.SetHasNavigationBar(this, false); // remove the title for the navigation pane, cause it takes up too much realestate
            settingsView = new Grid();

            Content = GetSettingsPage();
        }

        protected void RefreshSkinTone()
        {
            skinToneSample.Source = (Application.Current as App).imageLoader.Image("self01.png").Source;
        }

        protected void PreviewGrid()
        {
            //Delete the old grid, if it is there.
            innerSGDPreview.Children.Clear();
            innerSGDPreview.ColumnDefinitions.Clear();
            innerSGDPreview.RowDefinitions.Clear();


            //Build the new grid
            int colCount;
            if (Int32.TryParse(columns.Text, out colCount))
            {
                ColumnDefinitionCollection tempCols = new ColumnDefinitionCollection();
                for (int i = 0; i < colCount; i++)
                {
                    tempCols.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }

                RowDefinitionCollection tempRows = new RowDefinitionCollection();
                double rowCount = colCount * 0.8; //0.8 is a constant, based on 6 columns showing 5 rows, or 0.8333

                for (int i = 0; i < Math.Floor(rowCount); i++)
                {
                    tempRows.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                innerSGDPreview.ColumnDefinitions = tempCols;
                innerSGDPreview.RowDefinitions = tempRows;

                //Add dummy buttons
                for (int y = 0; y < Math.Floor(rowCount); y++)
                {
                    for (int x = 0; x < colCount; x++)
                    {
                        if (x == 0 && (y == 0 || y == 1)) { innerSGDPreview.Children.Add(new BoxView { BackgroundColor = Color.Azure }, x, y); }
                        else if (x == colCount - 1 && (y == 0 || y == 1)) { innerSGDPreview.Children.Add(new BoxView { BackgroundColor = Color.Red }, x, y); }
                        else { innerSGDPreview.Children.Add(new BoxView { BackgroundColor = Color.WhiteSmoke }, x, y); } // using antique white so it is viewable against background
                    }
                }
            }
        }

        protected ScrollView GetSettingsPage()
        {
            ScrollView outerView = new ScrollView();
            Grid settingsPage = new Grid();
            skinToneSample = new Image();
            columns = new Entry();

            settingsPage.HorizontalOptions = LayoutOptions.FillAndExpand;

            settingsPage.ColumnDefinitions = new ColumnDefinitionCollection {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
            };
            settingsPage.RowDefinitions = new RowDefinitionCollection {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            };

            //Set up left side of Settings page
            StackLayout leftStack = new StackLayout();

            leftStack.Children.Add(new Label
            {
                Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigLanguageLabel,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center
            });


            //Fill in the language picker
            languagePicker = new Picker { Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigLanguageLabel };
            List<Languages> systemLanguages = (Application.Current as App).configuration.GetLanguages();
            int currentLanguageIndex = 0;

            for (int i = 0; i < systemLanguages.Count; i++)
            {
                if (systemLanguages[i].LanguageID == (Application.Current as App).configuration.GetLanguage())
                {
                    currentLanguageIndex = i; // this allows the picker to be set to the current system language.
                }
                languagePicker.Items.Add(systemLanguages[i].Language);
            }
            //Select the current language as the selected picker item
            languagePicker.SelectedIndex = currentLanguageIndex;

            leftStack.Children.Add(languagePicker);

            leftStack.Children.Add(new BoxView { BackgroundColor = Color.LightGray, HeightRequest = 2, WidthRequest = 50 });

            #region SkinTone Settings
            leftStack.Children.Add(new Label
            {
                Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigSkinToneLabel,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center
            });

            //Set Current Skin Tone Image
            RefreshSkinTone();

            skinToneSample.HorizontalOptions = LayoutOptions.Center;
            Grid skinToneBox = new Grid();
            skinToneBox.ColumnDefinitions = new ColumnDefinitionCollection {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            };
            skinToneBox.RowDefinitions = new RowDefinitionCollection {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            };
            skinToneBox.Children.Add(skinToneSample);
            // Add six skintone buttons
            Button toneA = new Button { BackgroundColor = Color.FromHex("FFDB5E") };
            Button toneB = new Button { BackgroundColor = Color.FromHex("F7DECE") };
            Button toneC = new Button { BackgroundColor = Color.FromHex("F3D2A2") };
            Button toneD = new Button { BackgroundColor = Color.FromHex("D5AB88") };
            Button toneE = new Button { BackgroundColor = Color.FromHex("AF7E57") };
            Button toneF = new Button { BackgroundColor = Color.FromHex("7C533E") };

            toneA.Clicked += ToneA_Clicked;
            toneB.Clicked += ToneB_Clicked;
            toneC.Clicked += ToneC_Clicked;
            toneD.Clicked += ToneD_Clicked;
            toneE.Clicked += ToneE_Clicked;
            toneF.Clicked += ToneF_Clicked;

            skinToneBox.Children.Add(toneA, 1, 0);
            skinToneBox.Children.Add(toneB, 2, 0);
            skinToneBox.Children.Add(toneC, 3, 0);
            skinToneBox.Children.Add(toneD, 4, 0);
            skinToneBox.Children.Add(toneE, 5, 0);
            skinToneBox.Children.Add(toneF, 6, 0);

            leftStack.Children.Add(skinToneBox);

            #endregion

            leftStack.Children.Add(new BoxView { BackgroundColor = Color.LightGray, HeightRequest = 2, WidthRequest = 50 });

            #region Section Enabler
            Grid enableSections = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition{ Width = new GridLength(5, GridUnitType.Star) },
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            showSGD = new CheckBox { Color = Color.Black, IsChecked = (Application.Current as App).configuration.GetShowPicto() };
            showDS = new CheckBox { Color = Color.Black, IsChecked = (Application.Current as App).configuration.GetDailyStories() };
            showSI = new CheckBox { Color = Color.Black, IsChecked = (Application.Current as App).configuration.GetSocialInteractions() };
            showPI = new CheckBox { Color = Color.Black, IsChecked = (Application.Current as App).configuration.GetPainIndicator() };
            showER = new CheckBox { Color = Color.Black, IsChecked = (Application.Current as App).configuration.GetEmotionalRegulation() };


            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigShowSectionLabel, FontSize = 20, FontAttributes = FontAttributes.Bold }, 1, 0);
            enableSections.Children.Add(showSGD, 0, 1);
            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationSGDLabel, FontSize = 14 }, 1, 1);
            enableSections.Children.Add(showDS, 0, 2);
            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationDailyStoriesLabel, FontSize = 14 }, 1, 2);
            enableSections.Children.Add(showSI, 0, 3);
            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationSocialIntLabel, FontSize = 14 }, 1, 3);
            enableSections.Children.Add(showPI, 0, 4);
            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationPainIndicatorLabel, FontSize = 14 }, 1, 4);
            enableSections.Children.Add(showER, 0, 5);
            enableSections.Children.Add(new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationEmotionalRegLabel, FontSize = 14 }, 1, 5);

            leftStack.Children.Add(enableSections);
            #endregion

            leftStack.Children.Add(new BoxView { BackgroundColor = Color.LightGray, HeightRequest = 2, WidthRequest = 50 });

            //Setting up right side of page
            StackLayout rightStack = new StackLayout();

            #region SGD Grid Size

            Button applyButton = new Button { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).InterfaceApplyButton };
            applyButton.Clicked += ApplyButton_Clicked;
            columns.Text = (Application.Current as App).configuration.GetPTSColumns().ToString();
            columns.Keyboard = Keyboard.Numeric;

            Grid outerSGDPreview = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            outerSGDPreview.Children.Add(new StackLayout
            {
                Children = {
                    new Label { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigColumnsLabel, FontSize = 14, FontAttributes = FontAttributes.Bold},
                    columns,
                    applyButton
                },
                VerticalOptions = LayoutOptions.End
            }, 0, 0);

            //Add the preview grid
            innerSGDPreview = new Grid();
            PreviewGrid(); // Initialize the preview grid
            outerSGDPreview.Children.Add(innerSGDPreview, 1, 0); // add the preview to the other side.

            rightStack.Children.Add(new Label
            {
                Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigSGGSizeLabel,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center
            });

            rightStack.Children.Add(outerSGDPreview);

            #endregion

            rightStack.Children.Add(new BoxView { BackgroundColor = Color.LightGray, HeightRequest = 2, WidthRequest = 50 });

            #region Commit Configuration Controls

            Button saveConfig = new Button { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigSaveButton, HorizontalOptions = LayoutOptions.FillAndExpand };
            Button resetPIN = new Button { Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).ConfigResetPinButton, HorizontalOptions = LayoutOptions.FillAndExpand };
            resetPIN.Clicked += ResetPIN_ClickedAsync;
            saveConfig.Clicked += SaveConfig_Clicked;

            Grid saveResetGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            saveResetGrid.Children.Add(saveConfig, 0, 0);
            saveResetGrid.Children.Add(resetPIN, 1, 0);

            rightStack.Children.Add(saveResetGrid);

            #endregion


            settingsPage.Children.Add(leftStack, 0, 0);
            settingsPage.Children.Add(rightStack, 1, 0);

            outerView.Content = settingsPage;

            return outerView;
        }

        private async void ResetPIN_ClickedAsync(object sender, EventArgs e)
        {
            string newPin = await DisplayPromptAsync(title: "Reset Pin", message: "Please enter your new PIN", accept: "Save", cancel: "Cancel", keyboard: Keyboard.Numeric);
            bool save = false;
            Regex rx = new Regex(@"^[0-9][0-9][0-9][0-9]$");

            if (string.IsNullOrEmpty(newPin))
            {
                await DisplayAlert("Error", "Please enter a 4 digit PIN", "Ok");
            }
            else if (!rx.IsMatch(newPin))
            {
                await DisplayAlert("Error", "Please enter a 4 digit PIN", "Ok");
            }
            else
            {
                save = true;
            }
            if (save)
            {
                (Application.Current as App).configuration.WritePIN(Int32.Parse(newPin));
            }
        }

        private void SaveConfig_Clicked(object sender, EventArgs e)
        {
            var config = (Application.Current as App).configuration;
            config.SetLanguage(languagePicker.SelectedItem.ToString());
            //Skin tone is already set
            int colCount;
            if (Int32.TryParse(columns.Text, out colCount)) config.SetPTSColumns(colCount);
            config.SetShowPicto(showSGD.IsChecked);
            config.SetDailyStories(showDS.IsChecked);
            config.SetSocialInteractions(showSI.IsChecked);
            config.SetPainIndicator(showPI.IsChecked);
            config.SetEmotionalRegulation(showER.IsChecked);

            config.WriteConfiguration();

            Application.Current.MainPage.Navigation.PopAsync(); // return to last page
        }

        private void ApplyButton_Clicked(object sender, EventArgs e)
        {
            int colCountCheck;
            if (Int32.TryParse(columns.Text, out colCountCheck))
            {
                if (colCountCheck < 4)
                {
                    columns.Text = "4"; // 4 is the minumum before the math breaks
                }
            }

            PreviewGrid(); // refreshes the grid with the new column count
        }

        private void ToneF_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(6);
            RefreshSkinTone();
        }

        private void ToneE_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(5);
            RefreshSkinTone();
        }

        private void ToneD_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(4);
            RefreshSkinTone();
        }

        private void ToneC_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(3);
            RefreshSkinTone();
        }

        private void ToneB_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(2);
            RefreshSkinTone();
        }

        private void ToneA_Clicked(object sender, EventArgs e)
        {
            (Application.Current as App).configuration.SetSkinTone(1);
            RefreshSkinTone();
        }
    }
}