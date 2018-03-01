﻿using System;
using Xamarin.Forms;
using BookSync.Models;
using BookSync.Helpers;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookSync.ViewModels
{
    public class SelectFacebookUserViewModel : BaseViewModel
    {
        public event EventHandler Cancel;
        public event EventHandler<FacebookUser> FacebookUserSelected;

        public FacebookUser _selectedFacebookUser;
        public FacebookUser SelectedFacebookUser
        {
            get
            {
                return _selectedFacebookUser;    
            }
            set
            {
                SetProperty(ref _selectedFacebookUser, value);
            }
        }

        private ObservableCollection<FacebookUser> _facebookUsers = new ObservableCollection<FacebookUser>();
		public ObservableCollection<FacebookUser> FacebookUsers
		{
			get
			{
				return _facebookUsers;
			}
			set
			{
				SetProperty(ref _facebookUsers, value);
			}
		}

        private bool _facebookUsersLoaded = false;  

        public SelectFacebookUserViewModel(Page page) : base(page)
        {
        }

		private void LoadFacebookUsers()
		{
            if (_facebookUsersLoaded && FacebookUsers.Any())
                return;

            _facebookUsersLoaded = true;

            FacebookUsers.Clear();

            if (BookSyncContactsHelper.FacebookUsers == null)
                return;

            foreach (var facebookUser in BookSyncContactsHelper.FacebookUsers.OrderBy(o => o.FacebookName))
            {
                FacebookUsers.Add(facebookUser);
            }
		}

		Command loadFacebookUsersCommand;
		public ICommand LoadFacebookUsersCommand
		{
			get
			{
				return loadFacebookUsersCommand ?? (loadFacebookUsersCommand =
														 new Command(async () => await ExecuteLoadFacebookUsersCommand()));
			}
		}

		public async Task ExecuteLoadFacebookUsersCommand()
		{
			LoadFacebookUsers();
		}

		Command cancelCommand;
		public ICommand CancelCommand
		{
			get
			{
				return cancelCommand ?? (cancelCommand =
														 new Command(async () => await ExecuteCancelCommand()));
			}
		}

		public async Task ExecuteCancelCommand()
		{
            Cancel?.Invoke(null, new EventArgs());
		}

		Command selectCommand;
		public ICommand SelectCommand
		{
			get
			{
				return selectCommand ?? (selectCommand = new Command(async () => await ExecuteSelectCommand()));
			}
		}

		public async Task ExecuteSelectCommand()
		{
            if (SelectedFacebookUser == null)
                return; 
            
            FacebookUserSelected?.Invoke(null, SelectedFacebookUser);

            SelectedFacebookUser = null;
		}
    }
}
