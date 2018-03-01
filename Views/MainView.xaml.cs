using System;
using BookSync.ViewModels;
using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class MainView : MasterDetailPage
    {
        MainViewViewModel viewModel;

        public MainView()
        {
            InitializeComponent();

            this.Master = new MenuView();
            this.Detail = new NavigationPage(App.ContactsToSyncPage);

            this.BindingContext = viewModel = new MainViewViewModel(this);
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteLoadAllContactsCommand();
		}
    }
}
