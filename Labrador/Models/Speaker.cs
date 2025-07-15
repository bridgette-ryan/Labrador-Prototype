using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Labrador.Models
{

    public class Speaker
    {
        List<Word> speakerWords; 

        public readonly Label speakerLabel;


        public Speaker ()
        {
            speakerWords = new List<Word>();

            speakerLabel = new Label();
            speakerLabel.VerticalTextAlignment = TextAlignment.Center;
            speakerLabel.HorizontalTextAlignment = TextAlignment.Center;
            speakerLabel.FontAttributes = FontAttributes.Bold;
            speakerLabel.FontSize = 20;
        }

        public int SentenceSize()
        {
            return speakerWords.Count;
        }

        public Word LastWord ()
        {
            return speakerWords[speakerWords.Count - 1];
        }

        public int GetPreviousWordID()
        {
            if(speakerWords.Count > 0)
            {
                return speakerWords[speakerWords.Count - 1].wordID;
            }
            return 0;
        }

        void SetSentence()
        {
            string temp = "";
            foreach (Word w in speakerWords) temp += " " + w.word;
            speakerLabel.Text = temp.ToUpper();
        }

        public void AddWord(Word word)
        {
            speakerWords.Add(word);
            SetSentence();
        }

        public void RemoveWord()
        {
            if (speakerWords.Count > 0)
            {
                speakerWords.RemoveAt(speakerWords.Count - 1);
            }
            SetSentence();
        }

        public void ClearSentence()
        {
            speakerWords.Clear();
            SetSentence();
        }
        public async Task SpeakSentence()
        {
            //Build sentence into string.

            string audioString = "";

            foreach (Word w in speakerWords)
            {
                if(String.IsNullOrEmpty(w.phonetic))
                {
                    audioString += " " + w.word;
                }
                else
                {
                    audioString += " " + w.phonetic;
                }
            }


            if(audioString != "")
            {
                await TextToSpeech.SpeakAsync(audioString);
            }
        }
    }
}
