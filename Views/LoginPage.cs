﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using BookSync.CustomControls;

namespace BookSync.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

			App.PostSuccessFacebookAction = token =>
             {
                Application.Current.MainPage = new MainView();
             };
		}
    }
}
