using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using Xamarin.Forms;
using BookSync.Models;
using BookSync.Helpers;
using System.Linq;

namespace BookSync.Views
{
    public partial class UnMatchedContactsView : ContentPage
    {
        UnMatchedViewModel viewModel;
        SelectFacebookUserView selectFBUserPage;

        bool hasCancelled;

        public UnMatchedContactsView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new UnMatchedViewModel(this);

			selectFBUserPage = new SelectFacebookUserView();

			selectFBUserPage.CloseDialog += SelectFBUserPage_CloseDialog;
            selectFBUserPage.FacebookUserSelected += SelectFBUserPage_FacebookUserSelected;

            viewModel.ShowToSyncClicked += (sender, e) =>
            {
                ((MasterDetailPage)Parent.Parent).Detail = new NavigationPage(App.ContactsToSyncPage);
            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            if(!hasCancelled)
                await viewModel.ExecuteLoadUnMatchedContactsCommand();
            else
                hasCancelled = false;
		}

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
			//show the list of fb users modally 
			if (listView.SelectedItem != null)
			{
				await Navigation.PushModalAsync(selectFBUserPage);
			}
        }

        async void SelectFBUserPage_CloseDialog(object sender, EventArgs e)
        {
            hasCancelled = true;
            await Navigation.PopModalAsync();
            listView.SelectedItem = null;
		}

        async void SelectFBUserPage_FacebookUserSelected(object sender, Models.FacebookUser e)
        {
            var selectedContact = (PhoneContact)listView.SelectedItem;

            //get the booksync contact
            var booksyncContact = BookSyncContactsHelper.BookSyncContacts.FirstOrDefault(f => 
                                                                                         f.PhoneContactId == 
                                                                                         selectedContact.PhoneContactId);

            booksyncContact.FacebookUserId = e.ID;
            booksyncContact.FacebookFirstName = e.FirstName;
            booksyncContact.FacebookLastName = e.LastName;
            booksyncContact.FacebookImageSmallUrl = e.ProfileImageSmallUrl;
            booksyncContact.FacebookImageLargeUrl = e.ProfileImageLargeUrl;

            await BookSyncContactsHelper.SaveBookSyncContacts();

            viewModel.UnMatchedContacts.Remove(viewModel.UnMatchedContacts.
                                               FirstOrDefault(w => w.PhoneContactId == selectedContact.PhoneContactId));
            
            await Navigation.PopModalAsync();

            viewModel.UnMatchedContactsChanged();

            listView.SelectedItem = null;
		}
    }
}
