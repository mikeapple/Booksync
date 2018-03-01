using System;
using Xamarin.Forms;
using BookSync.Models;
using System.Collections.Generic;
using BookSync.Helpers;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BookSync.ViewModels
{
    public class UnMatchedViewModel : BaseViewModel
    {
        public event EventHandler ShowToSyncClicked;

		private ObservableCollection<PhoneContact> _unMatchedContacts = new ObservableCollection<PhoneContact>();
		public ObservableCollection<PhoneContact> UnMatchedContacts
		{
			get
			{
				return _unMatchedContacts;
			}
			set
			{
				SetProperty(ref _unMatchedContacts, value);
			}
		}

        public bool HasContacts
        {
            get
            {
                if (!hasLoaded)
                    return false;

                return UnMatchedContacts != null ? UnMatchedContacts.Any() : false;
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

        public UnMatchedViewModel(Page page) : base(page)
        {
            
        }

		private void LoadUnMatchedContacts()
        {
            hasLoaded = false;

            UnMatchedContacts.Clear();
            //go through the collection of phone contacts and check if there's a match in the booksync contacts and if not then 
            //add it to the list

            if (BookSyncContactsHelper.PhoneContacts == null || !BookSyncContactsHelper.PhoneContacts.Any())
                return;

            if(BookSyncContactsHelper.BookSyncContacts == null || !BookSyncContactsHelper.BookSyncContacts.Any())
            {
                foreach (var phoneContact in BookSyncContactsHelper.PhoneContacts.OrderBy(o => o.PhoneName).ToList())
                {
                    UnMatchedContacts.Add(phoneContact);
                }

                return;
            }

            foreach (var phoneContact in BookSyncContactsHelper.PhoneContacts.OrderBy(o => o.PhoneName).ToList())
            {
                if(BookSyncContactsHelper.BookSyncContacts.FirstOrDefault(f => f.PhoneContactId == phoneContact.PhoneContactId && 
                                                                          string.IsNullOrEmpty(f.FacebookUserId)) != null)
                {
                    UnMatchedContacts.Add(phoneContact);
                }
            }

            hasLoaded = true;
            OnPropertyChanged(nameof(HasContacts));
            OnPropertyChanged(nameof(HasContactsToSync));
		}

        public void UnMatchedContactsChanged()
        {
			OnPropertyChanged(nameof(UnMatchedContacts));
            hasLoaded = true;
            OnPropertyChanged(nameof(HasContacts));
            OnPropertyChanged(nameof(HasContactsToSync));

            int toSyncCount = BookSyncContactsHelper.BookSyncContacts.Count(c => !string.IsNullOrEmpty(c.FacebookUserId) && !c.InSync);
            SubInformation = $"{toSyncCount} Contacts Ready to Sync.";
		}

		Command loadContactsCommand;
		public ICommand LoadContactsCommand
		{
			get
			{
				return loadContactsCommand ?? (loadContactsCommand =
														 new Command(async () => await ExecuteLoadUnMatchedContactsCommand()));
			}
		}

		public async Task ExecuteLoadUnMatchedContactsCommand()
		{
			LoadUnMatchedContacts();
		}

        Command syncButtonCommand;
        public ICommand SyncButtonCommand
        {
            get
            {
                return syncButtonCommand ?? (syncButtonCommand = new Command(async () => await ExecuteSyncButtonCommand()));
            }
        }

        public async Task ExecuteSyncButtonCommand()
        {
            ShowToSyncClicked?.Invoke(this, new EventArgs());
        }
    }
}
