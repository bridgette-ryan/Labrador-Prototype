using System;
using SQLite;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using Labrador.Models;
using System.Linq;
using Android.Views;
using Android;

namespace Labrador.Droid
{
    [Activity(Label = "Labrador", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public override void OnBackPressed()
        {
            //Disable the gosh dang back button!!!!
            //base.OnBackPressed();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "config.db");

            /* This section is designed to determine if the application has been configured. If the existence of the file located in dbPath is false 
               then the system copies the default configuration from the Assets package and moves it to a writeable position on the device. Once there,
               the App is created using the initialize new setup constructor overload. */

            if (!File.Exists(dbPath))
            {
                //The standard file copy method described by the documentation has been deprecated, so I used the code from
                // https://stackoverflow.com/questions/58826984/cannot-read-sqlite-file-in-xamarin-android-project
                using (var binaryReader = new BinaryReader(Application.Context.Assets.Open("config.db")))
                {
                    using (var binaryWriter = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            binaryWriter.Write(buffer, 0, length);
                        }
                    }
                }
            }

            //Copy the speech icon -- Debug only
            using (var binaryReader = new BinaryReader(Application.Context.Assets.Open("Pictograms.zip")))
            {
                var zipPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "pictograms.zip");
                using (var binaryWriter = new BinaryWriter(new FileStream(zipPath, FileMode.Create)))
                {
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        binaryWriter.Write(buffer, 0, length);
                    }
                }
                //Unzip the file
                string outPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),"pictograms");
                ZipFile.ExtractToDirectory(zipPath, outPath, true);
            }

            //Attempting to copy any uncopied language databases to read/writable area.
            using (var db = new SQLiteConnection(dbPath))
            {
                var lang = from l in db.Table<DbLanguages>()
                                select l.LanguageDB;

                foreach (var l in lang)
                {
                    var languagePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), l);
                    if (!File.Exists(languagePath))
                    {
                        try
                        {
                            using (var binaryReader = new BinaryReader(Application.Context.Assets.Open(l)))
                            {
                                using (var binaryWriter = new BinaryWriter(new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), l), FileMode.Create)))
                                {
                                    byte[] buffer = new byte[2048];
                                    int length = 0;
                                    while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        binaryWriter.Write(buffer, 0, length);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Stub in case language DB doesn't exist.
                        }
                    }
                }
            }

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}