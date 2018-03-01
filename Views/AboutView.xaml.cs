using System;
using System.Collections.Generic;
using BookSync.ViewModels;
using Xamarin.Forms;

namespace BookSync.Views
{
    public partial class AboutView : ContentPage
    {
        AboutViewModel viewModel;

        public event EventHandler CloseDialog;

        public AboutView()
        {
            this.BindingContext = viewModel = new AboutViewModel(this);

            InitializeComponent();

            viewModel.CloseDialog += (sender, e) => {
                Navigation.PopModalAsync();
            };
        }
    }
}
