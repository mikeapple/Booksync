using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookSync.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
		public event EventHandler SelectionChange;

		private bool _isSelected = true;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value);
                SelectionChange?.Invoke(null, new EventArgs());
            }
        }

        public BaseModel()
        {
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

        public void IsSelectedChanged()
        {
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
