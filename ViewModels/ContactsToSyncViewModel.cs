using System;
using Xamarin.Forms;
using BookSync.Helpers;
using BookSync.Models;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace BookSync.ViewModels
{
    public class ContactsToSyncViewModel : BaseViewModel
    {
        public event EventHandler SyncContactsClicked;
        public event EventHandler ShowUnmatchedContactsClicked;
        public event EventHandler ShowAutoMatchContactsClicked;
        public event EventHandler SyncComplete;
        public event EventHandler<string> ErrorOccured;

        List<CancellationTokenSource> cancelTokens = new List<CancellationTokenSource>();
        bool HasErrored;
        private BookSyncContact CurrentContact { get; set; }

        private List<BookSyncContact> _contacts;
        public List<BookSyncContact> Contacts
        {
            get
            {
                return _contacts;   
            }
            set
            {
                SetProperty(ref _contacts, value);
            }
        }

        public bool HasContactsToSync
        {
            get
            {
                if (!hasLoaded || isSyncing)
                    return false;

                return Contacts != null ? Contacts.Any(a => a.IsSelected) : false;
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

        public bool HasContacts
        {
            get
            {
                if (!hasLoaded)
                    return false;

                return Contacts != null ? Contacts.Any() : false;
            }
        }

        private string _information;
        public string Information
        {
            get
            {
                return _information;
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

		private string unmatchedButtonText;
		public string UnmatchedButtonText
		{
			get
			{
                return "Match Unmatched Contacts"; // unmatchedButtonText;
			}
			set
			{
				SetProperty(ref unmatchedButtonText, value);
			}
		}

		private string autoMatchButtonText;
		public string AutoMatchButtonText
		{
			get
			{
                return "Auto Match Contacts"; // autoMatchButtonText;
			}
			set
			{
				SetProperty(ref autoMatchButtonText, value);
			}
		}

        private bool isSyncing;
        public bool IsSyncing
        {
            get
            {
                return isSyncing;
            }
            set
            {
                SetProperty(ref isSyncing, value);
                OnPropertyChanged(nameof(HasContactsToSync));
            }
        }

        private List<BookSyncException> syncExceptions;
        public List<BookSyncException> SyncExceptions
        {
            get
            {
                return syncExceptions;
            }
            set
            {
                SetProperty(ref syncExceptions, value);
            }
        }

        private List<BookSyncContact> contactsToSync;

        int completedContactsCount;
        int contactsToSyncCount;

        public ContactsToSyncViewModel(Page page) : base(page)
        {
            BookSyncContactsHelper.ContactsLoadedAndMatchced += BookSyncContactsHelper_ContactsLoadedAndMatchced;

            //set up to receive the events
            PhoneContactHelper.SyncSuccess += PhoneContactHelper_SyncSuccess;
            PhoneContactHelper.SyncError += PhoneContactHelper_SyncError;
        }

        private void BookSyncContactsHelper_ContactsLoadedAndMatchced(object sender, EventArgs e)
        {
            LoadContacts();
        }

        private void LoadContacts()
        {
            hasLoaded = false;
            OnPropertyChanged(nameof(HasContacts));

            Contacts = BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId) 
                                                                     && !w.InSync).ToList().OrderBy(o => o.PhoneName).ToList();

            foreach(var contact in Contacts)
            {
                contact.IsSelected = true;
            }

            if(!Contacts.Any())
            {
                Information = "There are no contacts to sync.";

                //populate the buttons text for unmatched and auto
                int unmatchedCount = BookSyncContactsHelper.BookSyncContacts.Count(c => string.IsNullOrEmpty(c.FacebookUserId));
                SubInformation = $"You have {unmatchedCount} unmatched contact{(unmatchedCount > 1 ? "s" : string.Empty)}.";
            }

            hasLoaded = true;
			OnPropertyChanged(nameof(HasContactsToSync));
			OnPropertyChanged(nameof(HasContacts));
            OnPropertyChanged(nameof(HasUnMatchedContacts));
        }

		Command loadContactsCommand;
		public ICommand LoadContactsCommand
		{
			get
			{
				return loadContactsCommand ?? (loadContactsCommand =
														 new Command(async () => await ExecuteLoadContactsCommand()));
			}
		}

		public async Task ExecuteLoadContactsCommand()
		{
			LoadContacts();
		}

		Command syncCommand;
		public ICommand SyncCommand
		{
			get
			{
				return syncCommand ?? (syncCommand = new Command(async () => await ExecuteSyncCommand()));
			}
		}

		public async Task ExecuteSyncCommand()
		{
            try
            {
                cancelTokens = new List<CancellationTokenSource>();
                SyncContactsClicked?.Invoke(this, new EventArgs());

                completedContactsCount = 0;
                SyncExceptions = new List<BookSyncException>();
                IsSyncing = true;
                HasErrored = false;

                //loop through each of the contacts and update each phone contact
                contactsToSync = BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId)
                                                                         && !w.InSync && w.IsSelected).ToList();
                contactsToSyncCount = contactsToSync.Count();

                foreach (var contact in contactsToSync)
                {
                    if (HasErrored)
                        break;

                    CurrentContact = contact;
                    
                    contact.IsSyncing = true;
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(TimeSpan.FromSeconds(15));
                    cancelTokens.Add(source);
                    Task task = Task.Run(() => PhoneContactHelper.SyncPhoneContact(contact, source.Token), source.Token);

                    await task;

                    //Remove this 
                    //throw new Exception("meh");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                IsSyncing = false;
                Information = "An unexpected error occured";
                HasErrored = true;
                CurrentContact.SyncFailed = true;

                ErrorOccured?.Invoke(this, Information);
            }
        }

        private void CheckForCompletedContacts()
        {
            completedContactsCount++;

            if (completedContactsCount == contactsToSyncCount)
            {
                AllContactsFinished();
            }
        }

        private async void AllContactsFinished()
        {
            IsSyncing = false;
            await BookSyncContactsHelper.SaveBookSyncContacts();

            //wait a second and then reset
            await Task.Delay(500);
            LoadContacts();

            SyncComplete?.Invoke(this, new EventArgs());
        }

        void PhoneContactHelper_SyncSuccess(object sender, Models.BookSyncContact e)
        {
            //Random rnd = new Random();
            //await Task.Delay(rnd.Next(8000));
            System.Diagnostics.Debug.WriteLine($"Success {e.FacebookName}");

            e.SyncCompleted = true;

            e.StoredFacebookImageUrl = e.FacebookImageLargeUrl;
            CheckForCompletedContacts();
        }

        void PhoneContactHelper_SyncError(object sender, Models.BookSyncException e)
        {
            System.Diagnostics.Debug.WriteLine($"Failure {e.AffectedContact.FacebookName}");
            SyncExceptions.Add(e);
            e.AffectedContact.SyncFailed = true;
            CheckForCompletedContacts();
        }

        Command cancelSyncCommand;
        public ICommand CancelSyncCommand
        {
            get
            {
                return cancelSyncCommand ?? (cancelSyncCommand = new Command(async () => await ExecuteCancelSyncCommand()));
            }
        }

        public async Task ExecuteCancelSyncCommand()
        {
            CancelSyncContacts();
        }

        private void CancelSyncContacts()
        {
            BookSyncContactsHelper.HasCancelled = true;
            IsSyncing = false;

            foreach (var token in cancelTokens)
            {
                token.Cancel();
            }
        }



        public void SelectionChanged()
        {
            OnPropertyChanged(nameof(HasContactsToSync));
        }

		Command unmatchedButtonCommand;
		public ICommand UnmatchedButtonCommand
		{
			get
			{
				return unmatchedButtonCommand ?? (unmatchedButtonCommand = new Command(async () => await ExecuteUmatchedButtonCommand()));
			}
		}

		public async Task ExecuteUmatchedButtonCommand()
		{
			ShowUnmatchedContactsClicked?.Invoke(this, new EventArgs());
		}

		Command autoMatchButtonCommand;
		public ICommand AutoMatchButtonCommand
		{
			get
			{
				return autoMatchButtonCommand ?? (autoMatchButtonCommand = new Command(async () => await ExecuteAutoMatchButtonCommand()));
			}
		}

		public async Task ExecuteAutoMatchButtonCommand()
		{
			ShowAutoMatchContactsClicked?.Invoke(this, new EventArgs());
		}

	}
}
