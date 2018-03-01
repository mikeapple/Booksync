using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using Xamarin.Forms;
using BookSync.Models;

namespace BookSync.Views
{
    public partial class MenuView : ContentPage
    {
        MenuViewModel viewModel;

        public MenuView()
        {
            InitializeComponent();
            BindingContext = viewModel = new MenuViewModel(this);

			var masterPageItems = new List<MasterPageItem>();
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Sync Contacts",
				IconSource = "ic_autorenew.png",
				TargetType = typeof(ContactsToSyncView)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Matched Contacts",
				IconSource = "ic_people.png",
				TargetType = typeof(MatchedContactsView)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Unmatched Contacts",
				IconSource = "ic_person.png",
				TargetType = typeof(UnMatchedContactsView)
			});
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Auto Match Contacts",
                IconSource = "ic_person_add.png",
                TargetType = typeof(AutoMatchContactsView)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "About",
                IconSource = "ic_help.png",
                TargetType = typeof(AboutView)
            });

			viewModel.MasterPageItems = masterPageItems;
		}

		void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as MasterPageItem;
            if (item == null)
                return;

            Page currentPage = null;
            if(item.TargetType == typeof(AutoMatchContactsView))
            {
                SetDetailPage(App.AutoMatchPage, item.TargetType);
            }
            else if (item.TargetType == typeof(ContactsToSyncView))
            {
                SetDetailPage(App.ContactsToSyncPage, item.TargetType);
            }
            else if (item.TargetType == typeof(MatchedContactsView))
            {
                SetDetailPage(App.MatchedContactsPage, item.TargetType);
            }
            else if (item.TargetType == typeof(UnMatchedContactsView))
            {
                SetDetailPage(App.UnMatchedContactsPage, item.TargetType);
            }
            else if (item.TargetType == typeof(AboutView))
            {
                DisplayPageModal(App.AboutPage, item.TargetType);
            }

			listView.SelectedItem = null;
			((MasterDetailPage)this.Parent).IsPresented = false;
		}

        private void SetDetailPage(Page page, Type type)
        {
            if (page == null)
                page = (Page)Activator.CreateInstance(type);
            
            ((MasterDetailPage)Parent).Detail = new NavigationPage(page);
        }

        private async void DisplayPageModal(Page page, Type type)
        {
            if (page == null)
                page = (Page)Activator.CreateInstance(type);

            await Navigation.PushModalAsync(page);
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteGetCurrentUserCommand();
		}
    }
}
