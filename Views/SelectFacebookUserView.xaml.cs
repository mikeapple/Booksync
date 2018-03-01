﻿using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using BookSync.Models;

using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class SelectFacebookUserView : ContentPage
    {
        SelectFacebookUserViewModel viewModel;

        public event EventHandler CloseDialog;
        public event EventHandler<FacebookUser> FacebookUserSelected;

        public SelectFacebookUserView()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new SelectFacebookUserViewModel(this);

            viewModel.Cancel += (sender, e) => 
            {
                CloseDialog?.Invoke(this, new EventArgs());
            };

            viewModel.FacebookUserSelected += (sender, e) => 
            {
                FacebookUserSelected?.Invoke(this, e);
            };
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            await viewModel.ExecuteLoadFacebookUsersCommand();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
			if (viewModel.SelectCommand.CanExecute(e))
			{
				viewModel.SelectCommand.Execute(e.SelectedItem);
			}

            listView.SelectedItem = null;
        }
    }
}
