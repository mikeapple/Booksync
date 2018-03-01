using System;
using System.Collections.ObjectModel;
using BookSync.Helpers;
using BookSync.Models;
using Xamarin.Forms;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using MvvmHelpers;
using System.Collections.Generic;

namespace BookSync.ViewModels
{
    public class MatchedContactsViewModel : BaseViewModel
    {
        public event EventHandler ShowUnmatchedContactsClicked;
        public event EventHandler ShowAutoMatchContactsClicked;

        //private ObservableRangeCollection<Grouping<string, BookSyncContact>>_matchedContacts = new ObservableRangeCollection<Grouping<string, BookSyncContact>>();
        //public ObservableRangeCollection<Grouping<string, BookSyncContact>> MatchedContacts
        //{
        //	get
        //	{
        //		return _matchedContacts;
        //	}
        //	set
        //	{
        //		SetProperty(ref _matchedContacts, value);
        //	}
        //}

        private ObservableCollection<BookSyncContact> _matchedContacts = new ObservableCollection<BookSyncContact>();
        public ObservableCollection<BookSyncContact> MatchedContacts
        {
            get
            {
                return _matchedContacts;
            }
            set
            {
                SetProperty(ref _matchedContacts, value);
            }
        }

        public bool HasContacts
        {
            get
            {
                if (!hasLoaded)
                    return false;

                return MatchedContacts != null ? MatchedContacts.Any() : false;
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

        public MatchedContactsViewModel(Page page) : base(page)
        {
        }

		private void LoadMatchedContacts()
		{
            hasLoaded = false;
            OnPropertyChanged(nameof(HasContacts));
			MatchedContacts.Clear();

            var matchedContacts = BookSyncContactsHelper.BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId))
                                                        .OrderBy(o => o.PhoneName).ToList();

            foreach (var contact in matchedContacts)
            {
                MatchedContacts.Add(contact);
            }

            //Only want to group by blank and A-Z

            //var sorted = from item in BookSyncContactsHelper.BookSyncContacts
            //where !string.IsNullOrEmpty(item.FacebookUserId)
            //orderby item.PhoneName
            //group item by ((item.PhoneName[0] > 64 && item.PhoneName[0] < 91) || (item.PhoneName[0] > 96 && item.PhoneName[0] < 123) ? 
            //               item.PhoneName[0].ToString().ToUpperInvariant() : string.Empty) into itemGroup
            //select new Grouping<string, BookSyncContact>(itemGroup.Key, itemGroup);

            //MatchedContacts.ReplaceRange(sorted);

            //int index = MatchedContacts.Count() - 1;
            //for (int i = 90; i > 64; i--)
            //{
            //    if (index < 0)
            //    {
            //        MatchedContacts.Insert(0, new Grouping<string, BookSyncContact>(((char)i).ToString(), new List<BookSyncContact>()));
            //    }
            //    else
            //    {
            //        var currentRecord = MatchedContacts[index];
            //        char character = currentRecord.Key[0];
            //        if (i != character)
            //        {
            //            //need to insert a blank record
            //            MatchedContacts.Insert(index + 1, new Grouping<string, BookSyncContact>(((char)i).ToString(), new List<BookSyncContact>()));
            //        }
            //        else
            //        {
            //            index--;
            //        }
            //    }
            //}

            CheckMatchedContacts();
        }

        public void CheckMatchedContacts()
        {
            if (!MatchedContacts.Any())
            {
                Information = "There are no matched contacts.";

                int unMatchedCount = BookSyncContactsHelper.BookSyncContacts.Where(w => string.IsNullOrEmpty(w.FacebookUserId)).Count();
                if (unMatchedCount > 0)
                {
                    SubInformation = $"You have {unMatchedCount} unmatched contacts.";
                }
            }

            hasLoaded = true;
            OnPropertyChanged(nameof(HasContacts));
            OnPropertyChanged(nameof(HasUnMatchedContacts));
        }

		Command loadMatchedContactsCommand;
		public ICommand LoadMatchedContactsCommand
		{
			get
			{
				return loadMatchedContactsCommand ?? (loadMatchedContactsCommand =
														 new Command(async () => await ExecuteLoadMatchedContactsCommand()));
			}
		}

		public async Task ExecuteLoadMatchedContactsCommand()
		{
			LoadMatchedContacts();
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
