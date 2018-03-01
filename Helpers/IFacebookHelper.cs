using System;
using BookSync.Models;
using System.Collections.Generic;
namespace BookSync
{
    public interface IFacebookHelper
    {
        event EventHandler CurrentUserLoaded;
        event EventHandler FacebookUsersLoaded;

        string GetFacebookAuthToken();
        void LoadCurrentUser();
		FacebookUser GetCurrentUser();
		void LoadFacebookUsers(string nextPageUrl = "");
		List<FacebookUser> GetFacebookUsers();
        void LogOut();
    }
}
