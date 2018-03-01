using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class SyncContactsView : ContentPage
    {
        public event EventHandler CloseDialog;

        SyncContactsViewModel viewModel;

        public SyncContactsView()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new SyncContactsViewModel(this);

			viewModel.CancelClicked += (sender, e) => CloseDialog?.Invoke(this, e);
            viewModel.CloseClicked += (sender, e) => CloseDialog?.Invoke(this, e);
        }


        protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.ExecuteSyncCommand();
		}
    }
}
