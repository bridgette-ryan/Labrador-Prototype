using Labrador.Models;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Labrador.ViewModels
{
    public class EditWord : ContentPage
    {
        public Image pictogramImage;
        public string pictogramFileName;
        Button browseButton, cameraButton, testSoundButton, saveButton, cancelButton;
        CheckBox personalInformation;
        Entry wordEntry, pronunciationEntry;
        Picker parentWordPicker;
        Word word;
        int parentID;
        int wordID;

        protected override void OnAppearing()
        {
            pictogramFileName = (Application.Current as App).fileNamePassBack;
            pictogramImage.Source = (Application.Current as App).imageLoader.Image(pictogramFileName).Source;
        }
        public EditWord(Word word, int parentWordID)
        {
            
            parentID = parentWordID; // So the clicker can access it.
            wordID = word.wordID;
            this.word = word;

            //Load image from existing word source.
            pictogramImage = new Image
            {
                Source = (Application.Current as App).imageLoader.Image(word.imageFile).Source
            };
            pictogramFileName = word.imageFile;


            NavigationPage.SetHasNavigationBar(this, false); // remove the title for the navigation pane, cause it takes up too much realestate

            Content =  EditPageGrid();
        }


        public Grid EditPageGrid()
        {
            //Shortcut this cause I am sick of typing out the whole thing!
            var ui = (Application.Current as App).uiTrans;
            var config = (Application.Current as App).configuration;

            //Initialise buttons and entries
            browseButton = new Button { Text = ui.GetTranslation(config.GetLanguage()).InterfaceBrowseButton };
            browseButton.Clicked += BrowseButton_Clicked;
            cameraButton = new Button { Text = ui.GetTranslation(config.GetLanguage()).InterfaceCameraButton };
            cameraButton.Clicked += CameraButton_ClickedAsync;

            wordEntry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Text = word.word };
            pronunciationEntry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Text = word.phonetic };
            testSoundButton = new Button { ImageSource = (Application.Current as App).imageLoader.ResourceImage("speak.png").Source };
            testSoundButton.Clicked += TestSoundButton_Clicked;


            /*
            //Initialise the picker
            List<Word> parentWordList = WordList.GetAllWords();

            parentWordPicker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand };
            parentWordPicker.SetBinding(Picker.ItemsSourceProperty, "Words");
            parentWordPicker.SetBinding(Picker.SelectedItemProperty, "SelectedWord");
            parentWordPicker.ItemDisplayBinding = new Binding("word");

            int parentIndex = 0;
            for (int i = 0; i < parentWordList.Count(); i++)
            {
                if (parentWordList[i].wordID == parentWordID) parentIndex = i;
            }

            parentWordPicker.ItemsSource = parentWordList;
            parentWordPicker.SelectedIndex = parentIndex;
            */

            //Initialize Save and Cancel buttons
            saveButton = new Button { Text = "SAVE NEEDS TRANS" };
            saveButton.Clicked += SaveButton_Clicked;
            cancelButton = new Button { Text = "CANCEL NEEDS TRANS" };
            cancelButton.Clicked += CancelButton_Clicked;

            //Initialise personal information checkbox.
            bool isChecked = false;
            if (word.personalInformation) isChecked = true;
            personalInformation = new CheckBox { IsChecked = isChecked };

            //Set up left hand side layout
            StackLayout leftSide = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children =
                {
                    pictogramImage,
                    browseButton,
                    cameraButton
                }
            };

            //Set up right side
            Grid rightSide = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(3,GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                        },
                RowDefinitions =
                        {
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)},
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)},
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)},
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)}
                        }
            };

            rightSide.Children.Add(new Label { Text = ui.GetTranslation(config.GetLanguage()).PTSEditInterfaceWordLabel }, 0, 0);
            rightSide.Children.Add(wordEntry, 1, 0);

            rightSide.Children.Add(new Label { Text = ui.GetTranslation(config.GetLanguage()).PTSEditInterfacePronounciationLabel }, 0, 1);
            rightSide.Children.Add(pronunciationEntry, 1, 1);

            rightSide.Children.Add(testSoundButton, 2, 1);

            rightSide.Children.Add(new Label { Text = ui.GetTranslation(config.GetLanguage()).PTSEditInterfacePersonalLabel }, 0, 2);
            rightSide.Children.Add(personalInformation, 1, 2);

            /*
            rightSide.Children.Add(new Label { Text = "PARENT WORD NEEDS TRANS" }, 0, 3);
            rightSide.Children.Add(parentWordPicker, 1, 3);
            */

            //Set up save and cancel buttons.
            Grid saveCancelGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                        },
                RowDefinitions =
                        {
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)}
                        }
            };

            saveCancelGrid.Children.Add(saveButton, 0, 0);
            saveCancelGrid.Children.Add(cancelButton, 1, 0);

            //Create outer container for it all
            Grid outerContainer = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(3,GridUnitType.Star) }
                        },
                RowDefinitions =
                        {
                            new RowDefinition{ Height = new GridLength(4,GridUnitType.Star)},
                            new RowDefinition{ Height = new GridLength(1,GridUnitType.Star)}
                        }
            };


            //Put it together and what do you got?
            outerContainer.Children.Add(leftSide, 0, 0);
            outerContainer.Children.Add(rightSide, 1, 0);
            outerContainer.Children.Add(saveCancelGrid, 1, 1);


            return outerContainer;
        }

        private async void CameraButton_ClickedAsync(object sender, EventArgs e)
        {
            MediaFile photo;
            try
            {
                photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });
                if (photo != null)
                {
                    var fileName = await DisplayPromptAsync("Name your image", "Please give a name for this picture", keyboard: Keyboard.Text);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (!(Application.Current as App).imageLoader.ImageFound(fileName))
                        {
                            (Application.Current as App).imageLoader.WriteImage(photo.GetStream(), fileName);

                            pictogramFileName = fileName;
                            pictogramImage.Source = (Application.Current as App).imageLoader.Image(fileName).Source;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public ScrollView BrowseImages()
        {
            List<string> fileNames = (Application.Current as App).imageLoader.GetImageFiles();

            Grid imageGrid = new Grid { HorizontalOptions = LayoutOptions.FillAndExpand };
            ColumnDefinitionCollection imageGridColumnDefinitions = new ColumnDefinitionCollection();
            RowDefinitionCollection imageGridRowDefinitions = new RowDefinitionCollection();
            int colCount = (Application.Current as App).configuration.GetPTSColumns();

            for (int i = 0; i < colCount; i++)
            {
                imageGridColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            int rowCount = Convert.ToInt32(Math.Ceiling((double)(fileNames.Count / colCount)));
            for (int i = 0; i < rowCount; i++) // Add the rows
            {
                imageGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            bool breaker = false;
            int index = 0;

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < colCount; x++)
                {
                    index++;

                    //Check to make sure haven't gone out of bounds of fileNames List.
                    if (index >= fileNames.Count)
                    {
                        breaker = true;
                        break;
                    }

                    var dbgFilename = fileNames[index];

                    SGDButton imageButton = new SGDButton { 
                        ImageSource = (Application.Current as App).imageLoader.Image(fileNames[index]).Source, 
                        word = new Word { imageFile = fileNames[index] }
                    };
                    imageButton.Clicked += ImageButton_Clicked;
                    imageGrid.Children.Add(imageButton, x, y);
                }
                if (breaker) break;
            }
            return new ScrollView { Content = imageGrid };
        }


        private async void BrowseButton_Clicked(object sender, EventArgs e)
        {
            this.Content = BrowseImages();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            //Store the variable
            pictogramFileName = ((SGDButton)sender).word.imageFile ;
            pictogramImage.Source = (Application.Current as App).imageLoader.Image(pictogramFileName).Source;

            this.Content = EditPageGrid();
        }


        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            var word = new Word(wordID, wordEntry.Text, pictogramFileName, personalInformation.IsChecked, pronunciationEntry.Text);
            word.UpdateWord(parentID);
            cancelButton.Text = "CLOSE NEEDS TRANS";
        }

        private async void TestSoundButton_Clicked(object sender, EventArgs e)
        {
            var vocal = wordEntry.Text;
            if (!string.IsNullOrEmpty(pronunciationEntry.Text)) vocal = pronunciationEntry.Text;
            await TextToSpeech.SpeakAsync(vocal);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync(); // return to last page
        }
    }
}