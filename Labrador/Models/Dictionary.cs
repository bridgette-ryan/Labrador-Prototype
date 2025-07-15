using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using SQLite;
using System.Linq;

namespace Labrador.Models
{
    public class Word
    {
        /* This class will be deprecated in future editions, with the methods below folded into
         * the DBWord class.
         */
        string dictDBPath ;

        public Word(int wordID, string word, string imageFile, bool personalInformation, string phonetic)
        {
            dictDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB());

            this.wordID = wordID;
            this.word = word;
            this.imageFile = imageFile;
            this.personalInformation = personalInformation;
            this.phonetic = phonetic;
        }

        public Word()
        {
            /* Method: Word() Constructor
             * 
             * This constructor is used specifically for adding a blank word to the dictionary prior to
             * writing it to the DB.
             * 
             * This method will be depracated in future versions, as dictionary editing will happen on the
             * companion app.
             */

            word = "";
            imageFile = "";
            personalInformation = true; // should a new word be created (calling this constructor) personal information is assumed by default
            phonetic = "";
        }

        public static void AssossciateParent(int wordID, int parentID)
        {
            /* Method: AssossciateParent(int,int)
             * This method was to be used in conjunction with the parent word picker, allowing the user to
             * change the parent word of a new or existing word. Due to limitations with the Picker class,
             * and problems with data binding, this ultimately not used.
             * 
             * This method will be depracated in future versions, as dictionary editing will happen on the
             * companion app.
             */
            using (var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB())))
            {
                var existingParent = from p in db.Table<DbTree>()
                                     where p.WordID.Equals(wordID)
                                     select p;

                if(existingParent.Count() > 0)
                {
                    db.Execute("UPDATE Tree SET ParentID = ? WHERE WordID =?", parentID, wordID);
                } 
                else
                {
                    db.Execute("INSERT INTO Tree(ParentID,WordID) VALUES(?, ?)", parentID, wordID);
                }
            }
        }

        public static void AssosciateWordPrediction(int preceedingID, int wordID)
        {
            /* Method: AssossciateWordPrediction(int,int)
             * 
             * This method is used specifically for the unsuccessful predictive text functionality that was attempted.
             * Due to the resource requirements in this prototype of the app, the predictive text was disabled, though
             * this function will likely come back into use in future editions of the app, once redesign moves the more
             * intensive processing usage functions to the companion app.
             */

            using (var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB())))
            {
                var assosciation = (from a in db.Table<DbPredictions>()
                                   where a.PreceedingWord.Equals(preceedingID) && a.WordID.Equals(wordID)
                                   select a).ToList();

                DbPredictions prediction = new DbPredictions { PreceedingWord = preceedingID, WordID = wordID };

                if (assosciation.Count() > 0) 
                {
                    /* //This update method is not working quite right
                    prediction.Count = assosciation.First().Count + 1;
                    db.Update(prediction);
                    */
                    prediction.Count = assosciation.FirstOrDefault().Count + 1;
                    db.Execute("UPDATE Predictions SET Count = ? WHERE PreceedingWord = ? AND WordID = ?",prediction.Count,prediction.PreceedingWord,prediction.WordID);
                }
                else
                {
                    prediction.Count = 1;
                    db.Insert(prediction);
                }
            }
        }

        public void DeleteWord()
        {
            using (var db = new SQLiteConnection(dictDBPath))
            {
                var word = from w in db.Table<DbWord>()
                           where w.WordID.Equals(this.wordID)
                           select w;
                db.Delete(word.First());
            }
        }

        public void UpdateWord(int parentID = 0)
        {
            using (var db = new SQLiteConnection(dictDBPath))
            {

                if (wordID == 0) this.wordID = -200; //Because 0 is still a word in the list. Duh!

                var dbWordValues = from w in db.Table<DbWord>()
                                where w.WordID.Equals(this.wordID)
                                select w;
                var checkCount = dbWordValues.Count();
                

                if(checkCount > 0)
                {
                    //db.Execute("UPDATE Word SET Word = ?, ImageFile = ?, PersonalInformation = ?, Phonetic = ? WHERE WordID = ?", this.word, this.imageFile, this.personalInformation, this.phonetic, dbWordValues.First().WordID);

                    var dbword = dbWordValues.First();

                    dbword.Word = this.word;
                    dbword.ImageFile = this.imageFile;
                    dbword.PersonalInformation = this.personalInformation;
                    dbword.Phonetic = this.phonetic;

                    db.Update(dbword);
                    db.Commit();
                }
                else
                {
                    db.Execute("INSERT INTO Word(Word,ImageFile,PersonalInformation,Phonetic) VALUES (?,?,?,?)",
                        this.word,
                        this.imageFile,
                        this.personalInformation,
                        this.phonetic);

                    db.Commit();

                    var lastID = (from l in db.Table<DbWord>()
                                 select l).Last().WordID;

                    db.Execute("INSERT INTO Tree(ParentID,WordID) VALUES (?,?)", parentID, lastID);
                    db.Commit();
                }
            }
        }

        public int wordID { get; set; }
        public string word { get; set; }
        public string imageFile { get; set; }
        public bool personalInformation { get; set; }
        public string phonetic { get; set; }

        public bool HasChildren()
        {
            /* Method: HasChildren()
             * 
             * This method checks to see if the current word has any children in the dictionary database.
             * This is how the app determines categories from words.
             */
            using (var db = new SQLiteConnection(dictDBPath))
            {
                var children = from c in db.Table<DbTree>()
                               where c.ParentID.Equals(this.wordID)
                               select c;
                if (children.Count() > 0) return true;
            }
            return false;
        }

        public static int GetParentID(Word word)
        {
            /* Method: GetParentID()
             * 
             * This method finds the parent word in DBTree, and returns an integer of the parent word.
             */
            var dictDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB());

            using (var db = new SQLiteConnection(dictDBPath))
            {
                return (from w in db.Table<DbTree>()
                        where w.WordID.Equals(word.wordID)
                        select w.ParentID).FirstOrDefault();
            }
        }

    }

    public class WordList
    {
        /* 
         * This class is the container for the DBTree of the current root id. This will be replaced in the final product by an
         * extended DBWord class that contains child words in a List<DBWord>
         */
        string dictDBPath;

        public WordList()
        {
            dictDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB());
        }

        public List<Word> GetWords(int root)
        {
            List<Word> words = new List<Word>();

            //Find selected language's dictionary.
            using (var db = new SQLiteConnection(dictDBPath))
            {
                 var wordlist = (from w in db.Table<DbWord>()
                                 join t in db.Table<DbTree>()
                                 on w.WordID equals t.WordID
                                 where t.ParentID.Equals(root)
                                 select new
                                 {
                                    WordID = t.WordID,
                                    Word = w.Word,
                                    ImageFile = w.ImageFile,
                                    PersonalInformation = w.PersonalInformation,
                                    Phonetic = w.Phonetic,
                                 }).ToList();

                foreach (var w in wordlist)
                {
                    words.Add(new Word(w.WordID, w.Word, w.ImageFile, w.PersonalInformation, w.Phonetic));
                }
            }
            return words;
        }

        public Word GetWord(int wordID)
        {
            using (var db = new SQLiteConnection(dictDBPath))
            {
                var dbWord = (from w in db.Table<DbWord>()
                               where w.WordID.Equals(wordID)
                               select w).First();
                return new Word(dbWord.WordID, dbWord.Word, dbWord.ImageFile, dbWord.PersonalInformation, dbWord.Phonetic);
            }
        }

        public static List<Word> GetAllWords()
        {
            /* This method is used to get a list of all words, specifically for when
             * the user wishes to set a specific word as the parent word (and thus
             * turn a word into a category.
             */

            var sDictDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), (Application.Current as App).configuration.GetLanguageDB());

            using (var db = new SQLiteConnection(sDictDBPath))
            {
                List<Word> words = new List<Word>();
                
                var dbWordList = (from w in db.Table<DbWord>()
                                  orderby w.Word ascending
                                  select w);

                foreach(var w in dbWordList)
                {
                    words.Add(new Word(w.WordID, w.Word, w.ImageFile, w.PersonalInformation, w.Phonetic));
                }

                return words;
            }
        }
    }
}
