using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using BookSync.Helpers;
using System.Windows.Input;

namespace BookSync.ViewModels
{
    public class MainViewViewModel : BaseViewModel
    {
        public MainViewViewModel(Page page) : base(page)
        {
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
            await BookSyncContactsHelper.LoadContacts();
        }
    }
}
