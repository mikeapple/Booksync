using System;
using System.Collections.Generic;
using BookSync.Models;
using BookSync.ViewModels;

using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class AutoMatchContactsView : ContentPage
    {
        AutoMatchContactsViewModel viewModel;

        public AutoMatchContactsView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new AutoMatchContactsViewModel(this);

            viewModel.ShowContactsToSyncClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.ContactsToSyncPage);
            };

            viewModel.ShowUnmatchedContactsClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.UnMatchedContactsPage);
            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteLoadAutoMatchedContactsCommand();
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            BookSyncContact contact = e.Item as BookSyncContact;

            if (contact == null)
                return;

            contact.IsSelected = !contact.IsSelected;
            contact.IsSelectedChanged();
            listView.SelectedItem = null;
        }
    }
}
