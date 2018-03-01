/*
 * Really Dirty for leaving this in but the syndication is now handled from within the other screen
 * this is just left in here in case I change my mind and I'm the only one 
 * to ever read this source code (If you've disassembled this then... why!?) 
 * 
 */


using System;
using System.Collections.ObjectModel;
using BookSync.Models;
using Xamarin.Forms;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BookSync.Helpers;
using System.Collections.Generic;

namespace BookSync.ViewModels
{
    public class SyncContactsViewModel : BaseViewModel
    {
		public event EventHandler CancelClicked;
		public event EventHandler CloseClicked;

		private List<BookSyncContact> _contactsToSync = new List<BookSyncContact>();
		public List<BookSyncContact> ContactsToSync
		{
			get
			{
				return _contactsToSync;
			}
			set
			{
				SetProperty(ref _contactsToSync, value);
			}
		}

        private string information;
        public string Information
        {
            get
            {
                return information;   
            }
            set
            {
                SetProperty(ref information, value);
            }
        }

        private bool syncCompleted;
        public bool SyncCompleted
        {
            get
            {
                return syncCompleted;
            }
            set
            {
                SetProperty(ref syncCompleted, value);
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

        private int completedContacts;

        public SyncContactsViewModel(Page page) : base(page)
        {
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
                completedContacts = 0;
                SyncCompleted = false;
                SyncExceptions = new List<BookSyncException>();

                //set up to receive the events
                PhoneContactHelper.SyncSuccess += PhoneContactHelper_SyncSuccess;
                PhoneContactHelper.SyncError += PhoneContactHelper_SyncError;

                //loop through each of the contacts and update each phone contact
                ContactsToSync = BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId)
                                                                         && !w.InSync && w.IsSelected).ToList();

                foreach (var contact in ContactsToSync)
                {
                    //await PhoneContactHelper.SyncPhoneContact(contact);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                syncCompleted = true;
                Information = "An unexpected error occured";
            }
        }

        private async void AllContactsFinished()
        {
            //once all completed then save the contacts
            await BookSyncContactsHelper.SaveBookSyncContacts();

            SyncCompleted = true;

            if (syncExceptions.Any())
            {
                Information = $"{syncExceptions.Count()} of {ContactsToSync.Count()} Contacts Failed to Update";
            }
            else
            {
                Information = $"{ContactsToSync.Count()} Contacts Updated";
            }
        }

        private async void CheckForCompletedContacts()
        {
            completedContacts++;

            if(completedContacts == ContactsToSync.Count())
            {
                AllContactsFinished();
            }
        }

        void PhoneContactHelper_SyncSuccess(object sender, Models.BookSyncContact e)
        {
            //Random rnd = new Random();
            //Task.Delay(rnd.Next(15000));

            e.StoredFacebookImageUrl = e.FacebookImageLargeUrl;
            CheckForCompletedContacts();
        }

        void PhoneContactHelper_SyncError(object sender, Models.BookSyncException e)
        {
            System.Diagnostics.Debug.WriteLine($"Failure {e.AffectedContact.FacebookName}");
            SyncExceptions.Add(e);
            CheckForCompletedContacts();
        }

        Command cancelSyncCommand;
		public ICommand CancelSyncCommand
		{
			get
			{
				return cancelSyncCommand ?? (cancelSyncCommand =
														 new Command(async () => await ExecuteCancelSyncCommand()));
			}
		}

		public async Task ExecuteCancelSyncCommand()
		{
			CancelSync();
		}

        private void CancelSync()
        {
            CancelClicked?.Invoke(this, new EventArgs());
        }

        Command completeSyncCommand;
        public ICommand CompleteSyncCommand
        {
            get
            {
                return completeSyncCommand ?? (completeSyncCommand =
                                                         new Command(async () => await ExecuteCompleteSyncCommand()));
            }
        }

        public async Task ExecuteCompleteSyncCommand()
        {
            CompleteSync();
        }

        private void CompleteSync()
        {
            CloseClicked?.Invoke(this, new EventArgs());
        }
    }
}
