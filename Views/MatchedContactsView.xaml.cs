using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using BookSync.Models;
using BookSync.Helpers;
using System.Linq;

using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class MatchedContactsView : ContentPage
    {
        MatchedContactsViewModel viewModel;

        public MatchedContactsView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new MatchedContactsViewModel(this);

            viewModel.ShowUnmatchedContactsClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.UnMatchedContactsPage);
            };

            viewModel.ShowAutoMatchContactsClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.AutoMatchPage);
            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteLoadMatchedContactsCommand();
		}

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var selectedMatchedContact = (BookSyncContact)e.Item;
            var bookSyncContact = BookSyncContactsHelper.BookSyncContacts.
                                                        FirstOrDefault(f => f.PhoneContactId == selectedMatchedContact.PhoneContactId);

            bookSyncContact.FacebookUserId = string.Empty;
            bookSyncContact.FacebookFirstName = string.Empty;
            bookSyncContact.FacebookLastName = string.Empty;
            bookSyncContact.FacebookImageSmallUrl = string.Empty;
            bookSyncContact.FacebookImageLargeUrl = string.Empty;
            bookSyncContact.StoredFacebookImageUrl = string.Empty;
            bookSyncContact.ShowFacebookImage = true;
            bookSyncContact.SyncFailed = false;
            bookSyncContact.SyncCompleted = false;
            bookSyncContact.IsSyncing = false;

            //var matchedGroup = viewModel.MatchedContacts.FirstOrDefault(f =>  
            //                      f.Items.FirstOrDefault(c => c.PhoneContactId == bookSyncContact.PhoneContactId) != null);
            
            //matchedGroup.Remove(matchedGroup.FirstOrDefault(f => f.PhoneContactId == bookSyncContact.PhoneContactId));

            //viewModel.MatchedContacts.Remove(matchedContact);
            await BookSyncContactsHelper.SaveBookSyncContacts();

            //viewModel.MatchedContacts.Remove(viewModel.MatchedContacts.FirstOrDefault(w => w.Key == selectedMatchedContact.PhoneName[0].ToString().ToUpperInvariant()).
            //FirstOrDefault(f => f.PhoneContactId == selectedMatchedContact.PhoneContactId).PhoneContactId);

            var matchedContact = viewModel.MatchedContacts.FirstOrDefault(f => f.PhoneContactId == bookSyncContact.PhoneContactId);
            viewModel.MatchedContacts.Remove(matchedContact);

            await BookSyncContactsHelper.SaveBookSyncContacts();

            viewModel.CheckMatchedContacts();
            listView.SelectedItem = null;
        }
    }
}
