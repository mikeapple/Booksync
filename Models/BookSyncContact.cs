using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BookSync.Models
{
    public class BookSyncContact : BaseModel
    {
        string _facebookUserId;
        public string FacebookUserId
        {
            get
            {
                return _facebookUserId;
            }

            set
            {
                SetProperty(ref _facebookUserId, value);
            }
        }

        string _facebookFirstName;
        public string FacebookFirstName
        {
            get
            {
                return _facebookFirstName;
            }

            set
            {
                SetProperty(ref _facebookFirstName, value);
            }
        }

        string _facebookLastName;
        public string FacebookLastName
        {
            get
            {
                return _facebookLastName;
            }

            set
            {
                SetProperty(ref _facebookLastName, value);
            }
        }

        string _facebookImageSmallUrl;
        public string FacebookImageSmallUrl
        {
            get
            {
                return _facebookImageSmallUrl;
            }

            set
            {
                _facebookImageSmallUrl = value;
                //SetProperty(ref _facebookImageUrl, value);
                OnPropertyChanged(nameof(FacebookProfileImage));
            }
        }

        string _facebookImageLargeUrl;
        public string FacebookImageLargeUrl
        {
            get
            {
                return _facebookImageLargeUrl;
            }

            set
            {
                _facebookImageLargeUrl = value;
                SetProperty(ref _facebookImageLargeUrl, value);
            }
        }

        public ImageSource FacebookProfileImage
		{
			get
			{
				if (string.IsNullOrEmpty(FacebookImageSmallUrl))
					return null;

                return UriImageSource.FromUri(new Uri(FacebookImageSmallUrl + $"&booksync"));
			}
		}

        string _phoneContactId;
        public string PhoneContactId
        {
            get
            {
                return _phoneContactId;
            }

            set
            {
                SetProperty(ref _phoneContactId, value);
            }
        }

        string _phoneFirstName;
        public string PhoneFirstName
        {
            get
            {
                return _phoneFirstName;
            }

            set
            {
                SetProperty(ref _phoneFirstName, value);
            }
        }

        string _phoneLastName;
        public string PhoneLastName
        {
            get
            {
                return _phoneLastName;
            }

            set
            {
                SetProperty(ref _phoneLastName, value);
            }
        }

        string _phoneImageBase64;
        public string PhoneImageBase64
        {
            get
            {
                return _phoneImageBase64;
            }

            set
            {
                _phoneImageBase64 = value;
                //SetProperty(ref _phoneImageBase64, value);
            }
        }

        string _storedFacebookImageUrl;
        public string StoredFacebookImageUrl
        {
            get
            {
                return _storedFacebookImageUrl;
            }

            set
            {
                SetProperty(ref _storedFacebookImageUrl, value);
            }
        }

        private bool isSyncing;
        public bool IsSyncing
        {
            get
            {
                return isSyncing;
            }
            set
            {
                if (value)
                {
                    ShowFacebookImage = false;
                    SyncCompleted = false;
                    SyncFailed = false;
                }
                SetProperty(ref isSyncing, value);
            }
        }

        private bool syncCompleted;
        public bool SyncCompleted
        {
            get
            {
                return syncCompleted;
            }
            set
            {
                IsSyncing = false;
                SetProperty(ref syncCompleted, value);
                OnPropertyChanged(nameof(SyncResultImage));
            }
        }

        private string syncResultImage;
        public string SyncResultImage
        {
            get
            {
                if(syncFailed)
                {
                    return "ic_error.png";
                }
                else
                {
                    return "ic_check_circle.png";
                }
            }
        }

        private bool syncFailed;
        public bool SyncFailed
        {
            get
            {
                return syncFailed;
            }
            set
            {
                OnPropertyChanged(nameof(SyncResultImage));
                IsSyncing = false;
                SetProperty(ref syncFailed, value);
            }
        }

        private bool showFacebookImage = true;
        public bool ShowFacebookImage
        {
            get
            {
                return showFacebookImage;
            }
            set
            {
                SetProperty(ref showFacebookImage, value);
            }
        }

        private bool showSyncResultImage = true;
        public bool ShowSyncResultImage
        {
            get
            {
                return showSyncResultImage;
            }
            set
            {
                SetProperty(ref showSyncResultImage, value);
            }
        }



        public BookSyncContact()
        {
        }

        public string FacebookName
        {
            get
            {
                return $"{FacebookFirstName} {FacebookLastName}";   
            }
        }

        public string PhoneName
        {
            get
            {
                return $"{PhoneFirstName} {PhoneLastName}";
            }
        }

		public bool DisplayPhoneImage
		{
			get
			{
				return !string.IsNullOrEmpty(PhoneImageBase64);
			}
		}

		public bool DisplayFacebookImage
		{
			get
			{
				return !string.IsNullOrEmpty(FacebookImageSmallUrl);
			}
		}

        public bool InSync
        {
            get
            {
                return FacebookImageLargeUrl == StoredFacebookImageUrl;
            }
        }

    }
}
