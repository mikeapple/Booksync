using System;
using Xamarin.Forms;
using BookSync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookSync.Helpers
{
    public static class FacebookHelper
	{
		public static EventHandler CurrentUserLoaded;
		public static EventHandler FacebooUsersLoaded;

		private static IFacebookHelper _facebookHelper = DependencyService.Get<IFacebookHelper>();
        public static List<FacebookUser> FacebookUsers;

		public static void LogOut()
        {
			_facebookHelper.LogOut();
            BookSyncContactsHelper.BookSyncContacts = null;
            BookSyncContactsHelper.SaveBookSyncContacts();
        }

        public static void LoadCurrentUser()
        {
            //TODO: Handle the error incase the profile can't be got
            _facebookHelper.CurrentUserLoaded += (sender, e) => 
                CurrentUserLoaded?.Invoke(null, new EventArgs());

            _facebookHelper.LoadCurrentUser();
        }

        public static FacebookUser GetCurrentUser()
        {
            return _facebookHelper.GetCurrentUser();
        }

		public static async Task LoadFacebookContacts()
		{
            _facebookHelper.FacebookUsersLoaded += (sender, e) =>
			{
                //The result will contain a list of users so we store them
                FacebooUsersLoaded?.Invoke(null, new EventArgs());
            };

            _facebookHelper.LoadFacebookUsers();
		}

        public static List<FacebookUser> GetFacebookUsers()
        {
            return _facebookHelper.GetFacebookUsers();
        }
    }
}
