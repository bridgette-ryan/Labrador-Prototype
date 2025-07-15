using Labrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Labrador.ViewModels
{

    public class PictogramToSpeech : ContentPage
    {
        Grid pictogramGrid;
        List<Word> currentWords;
        WordList wordList;
        Speaker speaker = new Speaker(); // must be public so reference can be passed through to SGDButtons on click.
        List<int> history;
        int root = 0;
        bool editMode;

        protected override void OnAppearing()
        {
            //This will refresh the grid on change of page for any reason (such as config changes, or edit changes)
            if(editMode)
            {
                RefreshEditGridAsync(root, pictogramGrid);
            }
            else
            {
                RefreshGridAsync(root, pictogramGrid);
            }
            base.OnAppearing();
        }

        public async void SpeakSingleWord(Word word)
        {
            //TO-DO: Add check to see if user wants to have words spoken as they are typed or not

            string vocal = word.word;
            if(!string.IsNullOrEmpty(word.phonetic))
            {
                vocal = word.phonetic;
            }
            await TextToSpeech.SpeakAsync(vocal);
        }

        public PictogramToSpeech()
        {
            Title = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).NavigationSGDLabel;
            IconImageSource = (Application.Current as App).imageLoader.ResourceImage("speech.png").Source;

            history = new List<int>();
            editMode = false;

            Button speakButton = new Button { ImageSource = (Application.Current as App).imageLoader.ResourceImage("speak.png").Source };

            speakButton.Clicked += SpeakButton_Clicked;
            Button backButton = new Button { Text = "<" };
            backButton.Clicked += BackButton_Clicked;

            Grid speakerGrid = new Grid
            {
                ColumnDefinitions = {
                            new ColumnDefinition {Width = new GridLength(5, GridUnitType.Star) },
                            new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star) }
                        },
                RowDefinitions = {new RowDefinition { Height = new GridLength(1,GridUnitType.Star)} }
            };
            speakerGrid.Children.Add(speaker.speakerLabel, 0, 0);
            speakerGrid.Children.Add(speakButton, 1, 0);
            speakerGrid.Children.Add(backButton, 2, 0);

            wordList = new WordList();
            Grid pageLayout = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions = {
                            new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto) }
                        },
                RowDefinitions = { 
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(5, GridUnitType.Star) }
                }
            };

            pictogramGrid = InitGrid(root);

            pageLayout.Children.Add(speakerGrid, 0, 0);
            pageLayout.Children.Add(new ScrollView { HorizontalOptions = LayoutOptions.FillAndExpand, Content = pictogramGrid }, 0, 1);

            Content = pageLayout;
        }

        #region Display Content Generators
        public Grid InitGrid(int root)
        {
            pictogramGrid = new Grid();
            RefreshGridAsync(root, pictogramGrid);

            return pictogramGrid;
        }

        private async Task RefreshGridAsync(int wordId, Grid pGrid)
        {
            pGrid.Children.Clear();
            pGrid.ColumnDefinitions.Clear();
            pGrid.RowDefinitions.Clear();
            pGrid.BackgroundColor = Color.White;


            currentWords = wordList.GetWords(wordId);
            var columnDefinitions = new ColumnDefinitionCollection();
            var rowDefinitions = new RowDefinitionCollection();

            //Create a list of SGD buttons so that they can be referenced
            List<SGDButton> pictograms = new List<SGDButton>();
            List<SGDButton> categories = new List<SGDButton>();

            for (int i = 0; i < currentWords.Count; i++)
            {
                string imageSource = currentWords[i].imageFile;
                string text = currentWords[i].word;

                if (String.IsNullOrEmpty(imageSource) || !(Application.Current as App).imageLoader.ImageFound(currentWords[i].imageFile))
                {
                    pictograms.Add(new SGDButton
                    {
                        Text = text,
                        BackgroundColor = Color.White,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        word = currentWords[i],
                    });
                }
                else
                {
                    pictograms.Add(new SGDButton
                    {
                        Text = text,
                        ImageSource = (Application.Current as App).imageLoader.Image(currentWords[i].imageFile).Source,
                        BackgroundColor = Color.White,
                        FontSize = 11,
                        word = currentWords[i]
                    });
                    pictograms[i].ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);
                }

                //Add onclick methods here.
                pictograms[i].Clicked += OnSGDButtonClicked;
            }

            foreach (var pic in pictograms)
            {
                if (pic.word.HasChildren())
                {
                    categories.Add(pic); // Add the category to the categories variable
                }
            }
            // Change background colour for categories
            foreach (var c in categories)
            {
                c.BackgroundColor = Color.Azure;
            }

            // remove categories from the pictograms
            pictograms.RemoveAll(x => x.word.HasChildren());


            //Set the number of columns required
            for (int i = 0; i < (Application.Current as App).configuration.GetPTSColumns(); i++)
            {
                columnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            pGrid.ColumnDefinitions = columnDefinitions;
            pGrid.HorizontalOptions = LayoutOptions.FillAndExpand;


            //Generate row numbers
            int reservedRowCount = Math.Max(4, categories.Count); //rows needed for reserved buttons
            for (int i = 0; i < reservedRowCount; i++)
            {
                rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Calculate number of available button spaces minus the reserved buttons
            int firstRowsCount = (((Application.Current as App).configuration.GetPTSColumns()) * reservedRowCount) - reservedRowCount;

            // Get difference in number of words in currentWords and current buttons available and calculate remaining rows required
            int extraRowCount = (currentWords.Count() - firstRowsCount + reservedRowCount) / ((Application.Current as App).configuration.GetPTSColumns()) + 1; // Add 1 incase of rounding down

            //Generate remaining rows
            for (int i = 0; i < extraRowCount; i++)
            {
                rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Add reserved buttons
            Button home = new Button
            {
                Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).PTSHomeButton,
                ImageSource = (Application.Current as App).imageLoader.ResourceImage("home.png").Source,
                BackgroundColor = Color.AliceBlue,
                FontSize = 11
            };
            home.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);
            Button edit = new Button
            {
                Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).PTSEditPageButton,

                ImageSource = (Application.Current as App).imageLoader.ResourceImage("cog.png").Source,
                BackgroundColor = Color.IndianRed,
                FontSize = 11
            };
            edit.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);

            Button back;

            if ((Application.Current as App).configuration.GetRTL())
            {
                back = new Button
                {
                    Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).PTSBackButton,

                    ImageSource = (Application.Current as App).imageLoader.ResourceImage("rightarrow.png").Source,
                    BackgroundColor = Color.AliceBlue,
                    FontSize = 11
                };
            }
            else
            {
                back = new Button
                {
                    Text = (Application.Current as App).uiTrans.GetTranslation((Application.Current as App).configuration.GetLanguage()).PTSBackButton,

                    ImageSource = (Application.Current as App).imageLoader.ResourceImage("leftarrow.png").Source,
                    BackgroundColor = Color.AliceBlue,
                    FontSize = 11
                };
            }
            back.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);

            edit.Clicked += Edit_ClickedAsync;
            home.Clicked += Home_Clicked;
            back.Clicked += Back_ClickedAsync;

            if ((Application.Current as App).configuration.GetRTL())
            {
                pGrid.Children.Add(home, 0, 0);
                pGrid.Children.Add(back, 0, 1);
                pGrid.Children.Add(edit, 0, 2);
            }
            else
            {
                int lastColumn = (Application.Current as App).configuration.GetPTSColumns() - 1;

                pGrid.Children.Add(home, lastColumn, 0);
                pGrid.Children.Add(back, lastColumn, 1);
                pGrid.Children.Add(edit, lastColumn, 2);
            }

            //Add first four row buttons
            //Check to see if word root has categories.
            int startCol = 0;
            if (categories.Count > 0) startCol = 1;
            int index = 0;

            bool breaker = false; // This helps break out of the double for loop
            for (int i = 0; i < reservedRowCount; i++) // This handles the rows
            {
                for (int j = startCol; j < (firstRowsCount / reservedRowCount); j++) // this handles the columns
                {
                    if (index < pictograms.Count())
                    {
                        int colPos = j;
                        // reverse the order items put in columns
                        if ((Application.Current as App).configuration.GetRTL())
                        {
                            colPos = (Application.Current as App).configuration.GetPTSColumns() - (j + 1);
                        }
                        pGrid.Children.Add(pictograms[index], colPos, i);
                        index++;
                    }
                    else { breaker = true; }
                }
                if (breaker) break; // If the number of buttons is smaller than the number of possible items in a row, break
            }

            if (!breaker)
            {

                breaker = false;
                //Generate the remaining buttons if breaker has not been set true.
                for (int i = 0; i < extraRowCount; i++)
                {
                    for (int j = 0; j < (firstRowsCount / reservedRowCount) + 1; j++)
                    {
                        pGrid.Children.Add(pictograms[index], j, i + reservedRowCount);
                        index++;
                        //Due to the rounding, there is a possible out of bounds error here, so break
                        if (index >= pictograms.Count)
                        {
                            breaker = true;
                            break;
                        }
                    }
                    if (breaker) break; // This breaks out of both loops, not just the bottom loop.
                }
            }

            //Add the category buttons
            int catColPos = 0;
            if ((Application.Current as App).configuration.GetRTL())
            {
                catColPos = (Application.Current as App).configuration.GetPTSColumns() - 1;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                pGrid.Children.Add(categories[i], catColPos, i);
            }
        }
        #endregion

        #region Edit Content Methods
        private SGDGrid EditGrid(Word word)
        {
            SGDGrid editGrid = new SGDGrid
            { 
                BackgroundColor = Color.White,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
                }
            };

            editGrid.word = word; // add word to the current SGDGrid

            //Set up left side
            SGDButton sGD = new SGDButton { BackgroundColor = Color.White, Text = word.word };
            if (String.IsNullOrEmpty(word.imageFile) || !(Application.Current as App).imageLoader.ImageFound(word.imageFile))
            {
                sGD.FontSize = 14;
                sGD.FontAttributes = FontAttributes.Bold;
            }
            else
            {
                sGD.ImageSource = (Application.Current as App).imageLoader.Image(word.imageFile).Source;
                sGD.FontSize = 11;
                sGD.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);
            }

            if (word.HasChildren())
            {
                sGD.BackgroundColor = Color.Azure;
            }

            sGD.word = word;
            editGrid.Children.Add(sGD,0,0);


            //Using SGD Buttons so that words are easily passed through to button click functions
            SGDButton editButton = new SGDButton { ImageSource = "pen.png" };
            SGDButton trashButton = new SGDButton { ImageSource = "trash.png" };
            editButton.word = word;
            editButton.Clicked += EditButton_Clicked;
            trashButton.word = word;
            trashButton.Clicked += TrashButton_Clicked;

            Grid buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
                },
            };

            buttonGrid.Children.Add(editButton, 0, 0);
            buttonGrid.Children.Add(trashButton, 0, 1);
            editGrid.Children.Add(buttonGrid,1,0);

            return editGrid;
        }

        private async Task RefreshEditGridAsync(int wordId, Grid pGrid)
        {
            pGrid.Children.Clear();
            pGrid.ColumnDefinitions.Clear();
            pGrid.RowDefinitions.Clear();
            pGrid.BackgroundColor = Color.Black;

            pGrid.HorizontalOptions = LayoutOptions.FillAndExpand;

            currentWords = wordList.GetWords(wordId);
            var columnDefinitions = new ColumnDefinitionCollection();
            var rowDefinitions = new RowDefinitionCollection();

            //Create a list of Grids so that they can be referenced
            List<SGDGrid> pictograms = new List<SGDGrid>();
            List<SGDGrid> categories = new List<SGDGrid>();

            for (int i = 0; i < currentWords.Count; i++)
            {
                if(currentWords[i].HasChildren())
                {
                    categories.Add(EditGrid(currentWords[i]));
                }
                else
                {
                    pictograms.Add(EditGrid(currentWords[i]));
                }
            }

            //Set the number of columns required
            for (int i = 0; i < (Application.Current as App).configuration.GetPTSColumns(); i++)
            {
                columnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            pGrid.ColumnDefinitions = columnDefinitions;


            /* Generate row numbers based on minimum of reserved words and categories.
             * Reserved buttons in this section: 
             *      Add Word
             *      Finish Editing
             */
            // Add reserved buttons
            Button close = new Button
            {
                Text = "CLOSE NEEDS TRANS",
                ImageSource = "close.png",
                BackgroundColor = Color.AliceBlue,
                FontSize = 11
            };
            close.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 10);
            close.Clicked += Close_ClickedAsync;
            Button addWord = new Button
            {
                Text = "ADD WORD NEEDS TRANS",
                ImageSource = "add.png",
                BackgroundColor = Color.AliceBlue,
                FontSize = 11
            };
            addWord.Clicked += AddWord_Clicked;

            int reservedRowCount = Math.Max(2, categories.Count); //rows needed for reserved buttons
            for (int i = 0; i < reservedRowCount; i++)
            {
                rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Calculate number of available button spaces minus the reserved buttons
            int firstRowsCount = (((Application.Current as App).configuration.GetPTSColumns()) * reservedRowCount) - reservedRowCount;

            // Get difference in number of words in currentWords and current buttons available and calculate remaining rows required
            int extraRowCount = (currentWords.Count() - firstRowsCount + reservedRowCount) / ((Application.Current as App).configuration.GetPTSColumns()) + 1; // Add 1 incase of rounding down

            //Generate remaining rows
            for (int i = 0; i < extraRowCount; i++)
            {
                rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            //Add reserved buttons
            if ((Application.Current as App).configuration.GetRTL())
            {
                pGrid.Children.Add(close, 0, 0);
                pGrid.Children.Add(addWord, 0, 1);
            }
            else
            {
                int lastColumn = (Application.Current as App).configuration.GetPTSColumns() - 1;
                pGrid.Children.Add(close, lastColumn, 0);
                pGrid.Children.Add(addWord, lastColumn, 1);
            }

            //Add first four row buttons
            //Check to see if word root has categories.
            int startCol = 0;
            if (categories.Count > 0) startCol = 1;
            int index = 0;

            bool breaker = false; // This helps break out of the double for loop
            for (int i = 0; i < reservedRowCount; i++) // This handles the rows
            {
                for (int j = startCol; j < (firstRowsCount / reservedRowCount); j++) // this handles the columns
                {
                    if (index < pictograms.Count())
                    {
                        int colPos = j;
                        // reverse the order items put in columns
                        if ((Application.Current as App).configuration.GetRTL())
                        {
                            colPos = (Application.Current as App).configuration.GetPTSColumns() - (j + 1);
                        }
                        pGrid.Children.Add(pictograms[index], colPos, i);
                        index++;
                    }
                    else { breaker = true; }
                }
                if (breaker) break; // If the number of buttons is smaller than the number of possible items in a row, break
            }

            if (!breaker)
            {
                breaker = false;
                //Generate the remaining buttons if breaker has not been set true.
                for (int i = 0; i < extraRowCount; i++)
                {
                    for (int j = 0; j < (firstRowsCount / reservedRowCount) + 1; j++) // include the reserve button column
                    {
                        pGrid.Children.Add(pictograms[index], j, i + reservedRowCount);
                        index++;
                        //Due to the rounding, there is a possible out of bounds error here, so break
                        if (index >= pictograms.Count)
                        {
                            breaker = true;
                            break;
                        }
                    }
                    if (breaker) break; // This breaks out of both loops, not just the bottom loop.
                }
            }

            //Add the category buttons
            int catColPos = 0;
            if ((Application.Current as App).configuration.GetRTL())
            {
                catColPos = (Application.Current as App).configuration.GetPTSColumns() - 1;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                pGrid.Children.Add(categories[i], catColPos, i);
            }
        }
        #endregion

        #region Button Click Methods

        private void Home_Clicked(object sender, EventArgs e)
        {
            history.Clear();
            root = 0;
            RefreshGridAsync(root, pictogramGrid);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            speaker.RemoveWord();
        }

        async void SpeakButton_Clicked(object sender, EventArgs e)
        {
            await speaker.SpeakSentence();
        }

        async void OnSGDButtonClicked(object sender, EventArgs args)
        {
            //Check if word has children.
            if (currentWords.Find(x => x.wordID == ((SGDButton)sender).word.wordID).HasChildren())
            {
                history.Add(((SGDButton)sender).word.wordID); // add new category to history
                root = ((SGDButton)sender).word.wordID;
                RefreshGridAsync(root, pictogramGrid);
            }
            else
            {
                //If no children, add word
                Word.AssosciateWordPrediction(
                    speaker.GetPreviousWordID(),
                    ((SGDButton)sender).word.wordID
                    );
                speaker.AddWord(((SGDButton)sender).word);
                SpeakSingleWord(((SGDButton)sender).word);
            }
        }

        private async void Back_ClickedAsync(object sender, EventArgs e)
        {
            if (history.Count == 0)
            {
                return;
            }

            //Remove the last item from history
            history.RemoveAt(history.Count - 1);
            if (history.Count != 0)
            {
                await RefreshGridAsync(history.Last(), pictogramGrid);
            }
            else
            {
                await RefreshGridAsync(0, pictogramGrid);
            }
        }

        private async void Edit_ClickedAsync(object sender, EventArgs e)
        {
            string securityPrompt = await DisplayPromptAsync("Security Check", "PIN", keyboard: Keyboard.Numeric);
            int PINNumber;
            int timer = 10000;

            if (Int32.TryParse(securityPrompt, out PINNumber))
            {
                if (PINNumber == (Application.Current as App).configuration.GetPIN())
                {
                    editMode = true;
                    RefreshEditGridAsync(root, pictogramGrid);
                }
                else
                {
                    ((Button)sender).IsEnabled = false;
                    await Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(timer);
                    }).Unwrap();
                    ((Button)sender).IsEnabled = true;
                }
            }
            else
            {
                ((Button)sender).IsEnabled = false;
                await Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(timer);
                }).Unwrap();
                ((Button)sender).IsEnabled = true;
            }
        }

        //Edit page buttons

        private async void AddWord_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditWord(new Word(), root));
        }

        private async void TrashButton_Clicked(object sender, EventArgs e)
        {
            var areYouSure = await DisplayAlert(title:"Delete entry",message: "Are you sure you want to delete this entry?", accept: "Yes", cancel: "No");
            if(areYouSure)
            {
                //((SGDButton)sender).word.
            }
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditWord(((SGDButton)sender).word,root));
        }

        private async void Close_ClickedAsync(object sender, EventArgs e)
        {
            editMode = false;
            await RefreshGridAsync(root, pictogramGrid);
        }

        #endregion
    }
}