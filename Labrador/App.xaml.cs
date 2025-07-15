using Xamarin.Forms;
using Labrador.ViewModels;
using Labrador.Models;

namespace Labrador
{
    public partial class App : Application
    {
        public Configuration configuration;
        public WordList wordList;
        public UITrans uiTrans;
        public ImageLoader imageLoader;

        public string fileNamePassBack { get; set; } // This is a very dirty way, fix it in final product!

        public App ()
        {
            InitializeComponent();
            configuration = new Configuration();
            uiTrans = new UITrans();
            imageLoader = new ImageLoader();

            if (configuration.IsConfigured())
            {
                MainPage = new NavigationPage(new NavigationPane());
            } else
            {
                MainPage = new NavigationPage(new InitializePageCS());
            }
        }


        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}
