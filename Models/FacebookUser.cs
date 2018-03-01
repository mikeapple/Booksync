using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BookSync.Models
{
    public class FacebookUser
	{
        public string ID { get; set; }
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageSmallUrl { get; set; }
        public string ProfileImageLargeUrl { get; set; }
        public List<FacebookUser> FacebookFriends { get; set; }

        public ImageSource ProfileImage
        {
            get
            {
                if (string.IsNullOrEmpty(ProfileImageSmallUrl))
                    return null;

                return UriImageSource.FromUri(new Uri(ProfileImageSmallUrl + "&booksync"));
            }
        }

		public bool DisplayFacebookImage
		{
			get
			{
				return !string.IsNullOrEmpty(ProfileImageSmallUrl);
			}
		}


		public string FacebookName
		{
			get
			{
				return $"{FirstName} {LastName}";
			}
		}

        public FacebookUser()
		{
            
        }
    }
}
