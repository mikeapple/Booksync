using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BookSync.Helpers;
using BookSync.Models;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace BookSync.ViewModels
{
    public class AutoMatchContactsViewModel : BaseViewModel
    {
        public event EventHandler ShowUnmatchedContactsClicked;
        public event EventHandler ShowContactsToSyncClicked;

		private ObservableCollection<BookSyncContact> _autoMatchedContacts = new ObservableCollection<BookSyncContact>();
		public ObservableCollection<BookSyncContact> AutoMatchedContacts
		{
			get
			{
				return _autoMatchedContacts;
			}
			set
			{
				SetProperty(ref _autoMatchedContacts, value);
			}
		}

        public bool HasMatchedContacts
        {
            get
            {
                return AutoMatchedContacts.Where(w => w.IsSelected).Any();
            }
        }

        public bool HasContacts
        {
            get
            {
                if (!hasLoaded)
                    return false;

                return AutoMatchedContacts != null ? AutoMatchedContacts.Any() : false;
            }
        }

        private string _information;
        public string Information
        {
            get
            {
                if (!HasUnMatchedContacts)
                    return "All your contacts are matched";

                return "No Matches Found."; //_information;
            }
            set
            {
                SetProperty(ref _information, value);
            }
        }

        private string subInformation;
        public string SubInformation
        {
            get
            {
                return subInformation;
            }
            set
            {
                SetProperty(ref subInformation, value);
            }
        }

        private string sub2Information;
        public string Sub2Information
        {
            get
            {
                return sub2Information;
            }
            set
            {
                SetProperty(ref sub2Information, value);
            }
        }

        public bool HasContactsToSync
        {
            get
            {
                if (!hasLoaded)
                    return false;

                int toSyncCount = BookSyncContactsHelper.BookSyncContacts.Where(w =>
                                                                                !string.IsNullOrEmpty(w.FacebookUserId) && !w.InSync).Count();

                return toSyncCount > 0;
            }
        }

        public bool HasUnMatchedContacts
        {
            get
            {
                if (!hasLoaded)
                    return false;

                int unMatchedCount = BookSyncContactsHelper.BookSyncContacts.Where(w => string.IsNullOrEmpty(w.FacebookUserId)).Count();

                return unMatchedCount > 0;
            }
        }


        public void ListUpdated()
        {
            OnPropertyChanged(nameof(AutoMatchedContacts));
        }

        public AutoMatchContactsViewModel(Page page) : base(page)
        {
        }

		private void LoadAutoMatchedContacts()
		{
			AutoMatchedContacts.Clear();
            HasLoaded = false;

            //no point continuing if either list is empty
            if (!BookSyncContactsHelper.FacebookUsers.Any() || !BookSyncContactsHelper.PhoneContacts.Any())
            {
                OnPropertyChanged(nameof(HasMatchedContacts));
                return;
            }

            var unMatchedContacts = BookSyncContactsHelper.BookSyncContacts.Where(w => string.IsNullOrEmpty(w.FacebookUserId)).OrderBy(o => o.PhoneName).ToList();

            foreach (var unMatchedContact in unMatchedContacts)
            {
                //lookup the facebook contacts for a match.
                FacebookUser matchedFacebookUser = null;

                //full name match
                matchedFacebookUser = BookSyncContactsHelper.FacebookUsers.FirstOrDefault(f => 
                                                                                          f.FacebookName.Replace(" ", string.Empty).Equals(unMatchedContact.PhoneName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));

                if(AddMatchedUser(unMatchedContact, matchedFacebookUser))
                    continue;
                
				//last name search 
                matchedFacebookUser = BookSyncContactsHelper.FacebookUsers.FirstOrDefault(f => 
                                                                                          f.LastName.Replace(" ", string.Empty).Equals(unMatchedContact.PhoneLastName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));

				if (AddMatchedUser(unMatchedContact, matchedFacebookUser))
					continue;

				//first name search
                matchedFacebookUser = BookSyncContactsHelper.FacebookUsers.FirstOrDefault(f => 
                                                                                          f.FirstName.Replace(" ", string.Empty).Equals(unMatchedContact.PhoneFirstName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));

				if (AddMatchedUser(unMatchedContact, matchedFacebookUser))
					continue;

			}

            AutoMatchedContactsChanged();
        }

        public void AutoMatchedContactsChanged()
        {
            OnPropertyChanged(nameof(AutoMatchedContacts));
            hasLoaded = true;
            OnPropertyChanged(nameof(HasContacts));
            OnPropertyChanged(nameof(HasContactsToSync));
            OnPropertyChanged(nameof(HasUnMatchedContacts));
            OnPropertyChanged(nameof(HasMatchedContacts));
            OnPropertyChanged(nameof(Information));

            int toSyncCount = BookSyncContactsHelper.BookSyncContacts.Count(c => !string.IsNullOrEmpty(c.FacebookUserId) && !c.InSync);
            SubInformation = $"{toSyncCount} Contacts Ready to Sync.";

            int unmatchedCount = BookSyncContactsHelper.BookSyncContacts.Count(c => string.IsNullOrEmpty(c.FacebookUserId));
            Sub2Information = $"You have {unmatchedCount} unmatched contact{(unmatchedCount > 1 ? "s" : string.Empty)}.";
        }

        private bool AddMatchedUser(BookSyncContact phoneContact, FacebookUser facebookUser)
        {
            if (facebookUser == null)
                return false;

            BookSyncContact matchedContact = new BookSyncContact();

			matchedContact.PhoneFirstName = phoneContact.PhoneFirstName;
			matchedContact.PhoneLastName = phoneContact.PhoneLastName;
			matchedContact.PhoneContactId = phoneContact.PhoneContactId;
			matchedContact.PhoneImageBase64 = phoneContact.PhoneImageBase64;

			matchedContact.FacebookFirstName = facebookUser.FirstName;
            matchedContact.FacebookLastName = facebookUser.LastName;
            matchedContact.FacebookUserId = facebookUser.ID;
            matchedContact.FacebookImageSmallUrl = facebookUser.ProfileImageSmallUrl;
            matchedContact.FacebookImageLargeUrl = facebookUser.ProfileImageLargeUrl;

            matchedContact.SelectionChange += (sender, e) => OnPropertyChanged(nameof(HasMatchedContacts));

            AutoMatchedContacts.Add(matchedContact);

            return true;
        }

		Command loadAutoMatchedContactsCommand;
		public ICommand LoadAutoMatchedContactsCommand
		{
			get
			{
				return loadAutoMatchedContactsCommand ?? (loadAutoMatchedContactsCommand =
														 new Command(async () => await ExecuteLoadAutoMatchedContactsCommand()));
			}
		}

		public async Task ExecuteLoadAutoMatchedContactsCommand()
		{
			LoadAutoMatchedContacts();
		}

		Command acceptMatchesCommand;
		public ICommand AcceptMatchesCommand
		{
			get
			{
				return acceptMatchesCommand ?? (acceptMatchesCommand =
														 new Command(async () => await ExecuteAcceptMatchesCommand()));
			}
		}

		public async Task ExecuteAcceptMatchesCommand()
		{
            //for each selected match update the master list of booksync contacts and save to json
            var selectedMatches = AutoMatchedContacts.Where(w => w.IsSelected);

            if (!selectedMatches.Any())
                return;

            //loop through the matches and store them then refresh the list.
            foreach (var selectedContact in selectedMatches)
            {
				var booksyncContact = BookSyncContactsHelper.BookSyncContacts.FirstOrDefault(f =>
																						f.PhoneContactId ==
																						selectedContact.PhoneContactId);

                booksyncContact.FacebookUserId = selectedContact.FacebookUserId;
                booksyncContact.FacebookFirstName = selectedContact.FacebookFirstName;
                booksyncContact.FacebookLastName = selectedContact.FacebookLastName;
                booksyncContact.FacebookImageSmallUrl = selectedContact.FacebookImageSmallUrl;
                booksyncContact.FacebookImageLargeUrl = selectedContact.FacebookImageLargeUrl;

				await BookSyncContactsHelper.SaveBookSyncContacts();
			}

            LoadAutoMatchedContacts();
        }

        Command syncButtonCommand;
        public ICommand SyncButtonCommand
        {
            get
            {
                return syncButtonCommand ?? (syncButtonCommand =
                                                         new Command(async () => await ExecuteSyncButtonCommand()));
            }
        }

        public async Task ExecuteSyncButtonCommand()
        {
            ShowContactsToSyncClicked?.Invoke(this, new EventArgs());
        }

        Command unmatchedButtonCommand;
        public ICommand UnmatchedButtonCommand
        {
            get
            {
                return unmatchedButtonCommand ?? (unmatchedButtonCommand =
                                                         new Command(async () => await ExecuteUnmatchedButtonCommandd()));
            }
        }

        public async Task ExecuteUnmatchedButtonCommandd()
        {
            ShowUnmatchedContactsClicked.Invoke(this, new EventArgs());
        }
    }
}
