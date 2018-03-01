using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BookSync.Helpers;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace BookSync.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public event EventHandler ContactsLoaded;

		private bool _showRetry = false;
		public bool ShowRetry
		{
			get
			{
				return _showRetry;
			}
			set
			{
				SetProperty(ref _showRetry, value);
			}
		}

		private bool _isRunning = true;
		public bool IsRunning
		{
			get
			{
				return _isRunning;
			}
			set
			{
				SetProperty(ref _isRunning, value);
			}
		}

        private string _loadingText = "Loading...";
        public string LoadingText
        {
            get
            {
                return _loadingText;
            }
            set
            {
                SetProperty(ref _loadingText, value);
            }
        }

        public LoadingViewModel(Page page) : base(page)
        {
            BookSyncContactsHelper.ContactLoadExceptionOccured += BookSyncContactsHelper_ContactLoadExceptionOccured;
            BookSyncContactsHelper.ContactsLoadedAndMatchced += (sender, e) => 
            {
                ContactsLoaded?.Invoke(null, new EventArgs());
                
            };
        }

        private void BookSyncContactsHelper_ContactLoadExceptionOccured(object sender, Exception e)
        {
            //TODO:// be more descriptive with the exceptin
            ShowRetry = true;
            IsRunning = false;
            LoadingText = "Unexpected Error Occured!";
        }

        Command loadAllContactsCommand;
        public ICommand LoadAllContactsCommand
        {
            get
            {
                return loadAllContactsCommand ?? (loadAllContactsCommand =
                                                         new Command(async () => await ExecuteLoadAllContactsCommand()));
            }
        }

        public async Task ExecuteLoadAllContactsCommand()
        {
            await LoadAllContacts();
        }

        private async Task LoadAllContacts()
        {
            IsRunning = true;

            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowRetry = true;
                LoadingText = "No Internet Connection.";
            }

            await BookSyncContactsHelper.LoadContacts();
        }

        Command retryCommand;
        public ICommand RetryCommand
		{
			get
			{
				return retryCommand ?? (retryCommand =
														 new Command(async () => await ExecuteRetryCommand()));
			}
		}

        public async Task ExecuteRetryCommand()
		{
			await LoadAllContacts();
		}

	}
}
