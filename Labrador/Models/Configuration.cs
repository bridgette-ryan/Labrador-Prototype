using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace Labrador.Models
{
    public struct Languages
    {
        public Languages(string l, int lid)
        {
            Language = l;
            LanguageID = lid;
        }

        public string Language { get; }
        public int LanguageID { get; }
    }

    public class Configuration
    {
        int languageID, ptsColumns, ptsRows, skinTone;
        bool showPicto, showDailyStories, showSocialInteractions, showPainIndicator, showEmotionalRegulation, rtl;
        string dictionary;

        bool configured;

        string configDBPath;

        #region Constructors
        public Configuration()
        {
            configured = false;
            configDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "config.db");

            languageID = 0;
            skinTone = 1; // set to 1 later, testing colour change right now
            ptsColumns = 6;
            ptsRows = 5;
            showPicto = true;
            showDailyStories = true;
            showSocialInteractions = false;
            showPainIndicator = false;
            showEmotionalRegulation = false;

            this.ReadConfiguration();
        }
        #endregion
        #region Setters and Getters
        public int GetLanguage() { return languageID; }
        public string GetLanguageDB() { return dictionary; }
        public void SetLanguage(string lang)
        {
            /*
             * Method: SetLanguage(string)
             *  This method finds the language by string in the database, and updates the configuration instance
             *  with the new language ID, dictionary location and whether the language is RtL or LtR.
             *  
             *  This method will be deprecated in the final product.
             */
            using (var db = new SQLiteConnection(configDBPath))
            {
                var language = from l in db.Table<DbLanguages>()
                               where l.LanguageName.Equals(lang)
                               select l;

                languageID = language.FirstOrDefault().LanguageID;
                dictionary = language.FirstOrDefault().LanguageDB;
                rtl = language.FirstOrDefault().RightToLeft;
            }
        }

        public Xamarin.Forms.Color GetSkinTone ()
        {
            /*
             * Method: GetSkinTone()
             *  This method checks the current instance of configuration for the enumerated skintone
             *  and returns as hexadecimal string to the calling method.
             *  
             *  This method will be deprecated in the final product.
             */
            switch (skinTone)
            {
                case 1:
                    return Xamarin.Forms.Color.FromHex("FFDB5E");
                case 2:
                    return Xamarin.Forms.Color.FromHex("F7DECE");
                case 3:
                    return Xamarin.Forms.Color.FromHex("F3D2A2");
                case 4:
                    return Xamarin.Forms.Color.FromHex("D5AB88");
                case 5:
                    return Xamarin.Forms.Color.FromHex("AF7E57");
                case 6:
                    return Xamarin.Forms.Color.FromHex("7C533E");
                default:
                    return Xamarin.Forms.Color.FromHex("FFDB5E");
            }
        }

        public void SetSkinTone(int skinTone) { this.skinTone = skinTone; }

        public int GetPTSColumns() { return ptsColumns; }
        public int GetPTSRows() { return ptsRows; }

        public void SetPTSColumns(int cols) { ptsColumns = cols; }
        public void SetPTSRows(int rows) { ptsRows = rows; }

        public bool GetShowPicto() { return showPicto; }
        public void SetShowPicto(bool setting) { showPicto = setting; }

        public bool GetDailyStories() { return showDailyStories; }
        public void SetDailyStories(bool setting) { showDailyStories = setting; }
        public bool GetSocialInteractions() { return showSocialInteractions; }
        public void SetSocialInteractions(bool setting) { showSocialInteractions = setting; }
        public bool GetPainIndicator() { return showPainIndicator; }
        public void SetPainIndicator(bool setting) { showPainIndicator = setting; }
        public bool GetEmotionalRegulation() { return showEmotionalRegulation; }
        public void SetEmotionalRegulation(bool setting) { showEmotionalRegulation = setting; }
        public bool GetRTL() { return rtl; }
        public bool IsConfigured() { return configured; }
        #endregion

        # region Configuration Management
        public void ReadConfiguration()
        {
            /*
             * Method: ReadConfiguration()
             *  This method pulls the configuration object settings from config.db, and loads them into the object.
             *  
             *  This method must be redesigned in final product.
             */

            using (var db = new SQLiteConnection(configDBPath))
            {
                var config = from c in db.Table<DbConfiguration>()
                             join l in db.Table<DbLanguages>()
                             on c.LanguageID equals l.LanguageID
                             select new
                             {
                                LanguageID = c.LanguageID,
                                SkinTone = c.SkinTone,
                                PTSColumns = c.PTSColumns,
                                PTSRows = c.PTSRows,
                                ShowSGD = c.ShowSGD,
                                ShowDS = c.ShowDS,
                                ShowSI = c.ShowSI,
                                ShowPI = c.ShowPI,
                                ShowER = c.ShowER,
                                LanguageDB = l.LanguageDB
                             };

                if(config.Count() > 0)
                {
                    languageID = config.FirstOrDefault().LanguageID;
                    skinTone = config.FirstOrDefault().SkinTone;
                    ptsColumns = config.FirstOrDefault().PTSColumns;
                    ptsRows = config.FirstOrDefault().PTSRows;
                    showPicto = config.FirstOrDefault().ShowSGD;
                    showDailyStories = config.FirstOrDefault().ShowDS;
                    showSocialInteractions = config.FirstOrDefault().ShowSI;
                    showPainIndicator = config.FirstOrDefault().ShowPI;
                    showEmotionalRegulation = config.FirstOrDefault().ShowER;
                    dictionary = config.FirstOrDefault().LanguageDB;

                    configured = true;
                }
            }
        }

        public void WritePIN(int pin)
        {

            /*
             * Method: GetSkinTone()
             *  This method stores the PIN number for the device, either by inserting or updating, depending on
             *  the existence of an already configured PIN.
             *  
             *  The security for the final product will be redesigned from this prototype, so this method
             *  will likely be deprecated in future versions.
             */

            using (var db = new SQLiteConnection(configDBPath))
            {
                var existingPin = (from p in db.Table<DBSecurity>()
                                    select p.PIN).ToList();

                if (existingPin.Count > 0) 
                {
                    DBSecurity entry = new DBSecurity { PIN = existingPin.FirstOrDefault() };
                    db.Execute("UPDATE security SET PIN = ? WHERE PIN = ?", pin, entry.PIN);
                    db.Commit();
                }
                else
                {
                    DBSecurity entry = new DBSecurity { PIN = pin };
                    db.Insert(entry);
                    db.Commit();
                }
            }
        }

        public int GetPIN()
        {
            //There should only be one PIN in the application, so returns the first record found.
            using (var db = new SQLiteConnection(configDBPath))
            {
                var existingPin = (from p in db.Table<DBSecurity>()
                                   select p.PIN).ToList();

                return existingPin.FirstOrDefault();
            }
        }

        public void WriteConfiguration()
        {
            /*
             * Method: WriteConfiguration()
             *  This method writes the current instance of configuration to the config.db database.
             *  
             *  This method will be altered in the final product to be accessible only through the
             *  companion Application (probably RPC or Remote Objects Calls)
             */
            using (var db = new SQLiteConnection(configDBPath))
            {
                DbConfiguration configuration = new DbConfiguration()
                {
                    ConfigID = 1,
                    LanguageID = this.languageID,
                    SkinTone = this.skinTone,
                    PTSColumns = this.ptsColumns,
                    PTSRows = this.ptsRows,
                    ShowSGD = this.showPicto,
                    ShowDS = this.showDailyStories,
                    ShowSI = this.showSocialInteractions,
                    ShowPI = this.showPainIndicator,
                    ShowER = this.showEmotionalRegulation
                };

                var existingConfig = (from c in db.Table<DbConfiguration>()
                                   select c.ConfigID).ToList();

                if (existingConfig.Count() == 0)
                {
                    db.Insert(configuration);
                }
                else
                {
                    db.Execute("UPDATE configuration SET languageID = ?, skintone = ?, PTSColumns = ?, PTSRows = ?, showSGD = ?, showDS = ?, showSI = ?, showPI = ?, showER = ? WHERE ConfigID = 1", configuration.LanguageID, configuration.SkinTone, configuration.PTSColumns, configuration.PTSRows, configuration.ShowSGD, configuration.ShowDS, configuration.ShowSI, configuration.ShowPI, configuration.ShowER );
                }
            }
        }
        #endregion
        public List<Languages> GetLanguages()
        {

            /*
             * Method: GetLanguages()
             *  This method is used for generating the language picker box, which is currently unused.
             *  
             *  This method will be deprecated in the final product, as languages will be set from the companion app.
             */

            List<Languages> lang = new List<Languages>();

            using (var db = new SQLiteConnection(configDBPath))
            {
                var languages = (from l in db.Table<DbLanguages>()
                                select l).ToList();

                foreach(var l in languages)
                {
                    lang.Add(new Languages(l.LanguageName, l.LanguageID));
                }
            }
            return lang;
        }
    }
}
