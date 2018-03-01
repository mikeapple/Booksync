using System;
namespace BookSync.Models
{
    public class PhoneContact
    {
        public string PhoneContactId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneImageBase64 { get; set; }

		public string PhoneName
		{
			get
			{
				return $"{FirstName} {LastName}";
			}
		}

		public PhoneContact()
        {
        }
    }
}
