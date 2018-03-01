using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace BookSync.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        internal readonly Page page;

        protected bool hasLoaded;
        public bool HasLoaded
        {
            get
            {
                return hasLoaded;
            }
            set
            {
                SetProperty(ref hasLoaded, value);
            }
        }

		public BaseViewModel(Page page)
		{
			this.page = page;
		}

		protected void SetProperty<T>(
			ref T backingStore, T value,
			[CallerMemberName]string propertyName = "",
			Action onChanged = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyName);
		}

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
