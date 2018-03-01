using System;
using Xamarin.Forms;
using BookSync.Views;
using System.Collections.Generic;
using BookSync.Models;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace BookSync
{
	public partial class App : Application
	{
		public static Action<string> PostSuccessFacebookAction { get; set; }

        private static Page autoMatchPage;
        public static Page AutoMatchPage
        {
            get 
            {
                if (autoMatchPage == null)
                    autoMatchPage = new AutoMatchContactsView();
                return autoMatchPage;
            }
        }

        private static Page contactsToSyncPage;
        public static Page ContactsToSyncPage
        {
            get
            {
                if (contactsToSyncPage == null)
                    contactsToSyncPage = new ContactsToSyncView();
                return contactsToSyncPage;
            }
        }

        private static Page matchedContactsPage;
        public static Page MatchedContactsPage
        {
            get
            {
                if (matchedContactsPage == null)
                    matchedContactsPage = new MatchedContactsView();
                return matchedContactsPage;
            }
        }

        private static Page unMatchedContactsPage;
        public static Page UnMatchedContactsPage
        {
            get
            {
                if (unMatchedContactsPage == null)
                    unMatchedContactsPage = new UnMatchedContactsView();
                return unMatchedContactsPage;
            }
        }

        private static Page aboutPage;
        public static Page AboutPage
        {
            get
            {
                if (aboutPage == null)
                { 
                    aboutPage = new AboutView(); 
                }
                return aboutPage;
            }
        }

        public App()
        {
            InitializeComponent();

            IFacebookHelper facebookHelper = DependencyService.Get<IFacebookHelper>();
            var fbKey = facebookHelper.GetFacebookAuthToken();

            if (string.IsNullOrEmpty(fbKey))
            {
                MainPage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = Color.FromHex("1f6cbc"),
                    BarTextColor = Color.White
                };
            }
            else
            {
                MainPage = new LoadingView();
                //MainPage = new MainView();
            }
        }

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
