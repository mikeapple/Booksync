using System;
using System.Collections.Generic;
using BookSync.Models;
using BookSync.ViewModels;
using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class ContactsToSyncView : ContentPage
    {
        ContactsToSyncViewModel viewModel;
        SyncContactsView syncContactsView = new SyncContactsView();

        private bool isSyncing;
       
        public ContactsToSyncView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new ContactsToSyncViewModel(this);
            viewModel.SyncContactsClicked += ViewModel_SyncContactsClicked;

            syncContactsView.CloseDialog += async (sender, e) =>
            {
                await viewModel.ExecuteLoadContactsCommand();
                await Navigation.PopModalAsync();
            };

            viewModel.ShowUnmatchedContactsClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.UnMatchedContactsPage);
            };

            viewModel.ShowAutoMatchContactsClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.AutoMatchPage);
            };

            viewModel.SyncComplete += (sender, e) => { isSyncing = false; };

            viewModel.ErrorOccured += async (sender, e) => 
            {
                await DisplayAlert("Error", e, "OK");
                await viewModel.ExecuteLoadContactsCommand();

            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteLoadContactsCommand();
		}

        async void ViewModel_SyncContactsClicked(object sender, EventArgs e)
        {
            isSyncing = true;
        }


        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isSyncing)
                return;

			BookSyncContact contact = e.Item as BookSyncContact;

			if (contact == null)
				return;

			contact.IsSelected = !contact.IsSelected;
			contact.IsSelectedChanged();
			ContactsListView.SelectedItem = null;

            viewModel.SelectionChanged();
        }
    }
}
