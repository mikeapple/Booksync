using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BookSync.Helpers;
using BookSync.Views;
using System.Collections.Generic;
using BookSync.Models;
using System.Linq;

namespace BookSync.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public event EventHandler<MasterPageItem> MenuItemClicked;

        private MasterPageItem _selectedMenuItem;
        public MasterPageItem SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;   
            }
            set
            {
                SetProperty(ref _selectedMenuItem, value);
            }
        }

        private List<MasterPageItem> _masterPageItems = new List<MasterPageItem>();
        public List<MasterPageItem> MasterPageItems
        {
            get
            {
                return _masterPageItems;   
            }
            set
            {
                SetProperty(ref _masterPageItems, value);
            }
        }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                SetProperty(ref _firstName, value);
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                SetProperty(ref _lastName, value);
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                SetProperty(ref _imageUrl, value);
            }
        }

        public string MatchedContactCount
        {
            get
            {
                if (BookSyncContactsHelper.BookSyncContacts == null)
                    return string.Empty;
                
                int count = 
                BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId)).Count();

                return $"{count} Matched";
            }
        }

		public string UnmatchedContactCount
		{
			get
			{
				if (BookSyncContactsHelper.BookSyncContacts == null)
					return string.Empty;

				int count =
				BookSyncContactsHelper.BookSyncContacts.Where(w => string.IsNullOrEmpty(w.FacebookUserId)).Count();

				return $"{count} Unmatched";
			}
		}

		public string ContactsToSyncCount
		{
			get
			{
				if (BookSyncContactsHelper.BookSyncContacts == null)
					return string.Empty;

				int count =
				BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId) && !w.InSync).Count();

				return $"{count} to Sync";
			}
		}

        public void UpdateCounts()
		{
			OnPropertyChanged(nameof(MatchedContactCount));
			OnPropertyChanged(nameof(UnmatchedContactCount));
			OnPropertyChanged(nameof(ContactsToSyncCount));
        }

        public MenuViewModel(Page page) : base(page)
        {
            BookSyncContactsHelper.ContactsSaved += (sender, e) => UpdateCounts();
        }

		#region Commands

		//Sync Contacts View
		Command getCurrentUserCommand;
		public ICommand GetCurrentUserCommand
		{
			get
			{
				return getCurrentUserCommand ?? (getCurrentUserCommand =
													  new Command(async () => await ExecuteGetCurrentUserCommand()));
			}
		}

		public async Task ExecuteGetCurrentUserCommand()
		{
            FacebookHelper.CurrentUserLoaded += (sender, e) => 
            {
				var facebookUser = FacebookHelper.GetCurrentUser();

				if (facebookUser == null)
					return;

				this.FirstName = facebookUser.FirstName;
				this.LastName = facebookUser.LastName;
				this.ImageUrl = facebookUser.ProfileImageSmallUrl;
            };

            FacebookHelper.LoadCurrentUser();
		}

		Command menuItemCommend;
		public ICommand MenuItemCommand
		{
			get
			{
				return menuItemCommend ?? (menuItemCommend =
													  new Command(async () => await ExecuteMenuItemCommand()));
			}
		}

		public async Task ExecuteMenuItemCommand()
		{
            MenuItemClicked?.Invoke(null, SelectedMenuItem);
		}

        //Logout
        Command logOutCommand;
		public ICommand LogOutCommand
		{
			get
			{
				return logOutCommand ?? (logOutCommand = new Command(async () => await ExecuteLogOutCommand()));
			}
		}

		public async Task ExecuteLogOutCommand()
		{
            FacebookHelper.LogOut();

            Settings.BookSyncContactsSettings = string.Empty;

			//Navigate back to the login page
			Application.Current.MainPage = new NavigationPage(new LoginPage())
			{
				BarBackgroundColor = Color.FromHex("1f6cbc"),
				BarTextColor = Color.White
			};
		}

		#endregion
    }
}
