using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace Labrador.Models
{
    public class UITrans
    {
        string transDBPath;
        //This is a key-value pair list of a key-value pair list. 
        //This enables the entire translation dictionary to remain in memory for easy swapping between languages.
        List<DBInterface> uiTranslation;

        public UITrans()
        {
            uiTranslation = new List<DBInterface>();
            transDBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "config.db");

            using (var db = new SQLiteConnection(transDBPath))
            {
                var interfaceLanguage = (from l in db.Table<DBInterface>() select l).ToList();

                for (int i = 0; i < interfaceLanguage.Count(); i++ )
                {
                    uiTranslation.Add(interfaceLanguage[i].ShallowCopy());
                }
            }
        }

        public DBInterface GetTranslation(int langID)
        {
            return uiTranslation.Find(x => x.LanguageID == langID);
        }
    }
}
