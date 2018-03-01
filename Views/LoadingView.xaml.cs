using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class LoadingView : ContentPage
    {
        LoadingViewModel viewModel;

        public LoadingView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new LoadingViewModel(this);

            viewModel.ContactsLoaded += (sender, e) => 
            {
                Application.Current.MainPage = new MainView();
            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteLoadAllContactsCommand();
		}
    }
}
