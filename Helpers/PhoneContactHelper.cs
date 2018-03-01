using System;
using System.Collections.Generic;
using BookSync.Models;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace BookSync.Helpers
{
    public static class PhoneContactHelper
    {
        public static event EventHandler PhoneContactsLoaded;
        public static event EventHandler FacebookUsersLoaded;
        public static event EventHandler<BookSyncContact> SyncSuccess;
        public static event EventHandler<BookSyncException> SyncError;

        private static IPhoneHelper phoneHelper;
        public static IPhoneHelper PhoneHelper
        {
            get
            {
                if(phoneHelper == null)
                {
                    phoneHelper = DependencyService.Get<IPhoneHelper>();

                    phoneHelper.SyncSuccess += (object sender, BookSyncContact e) =>
                    {
                        SyncSuccess?.Invoke(sender, e);
                    };

                    phoneHelper.SyncError += (object sender, BookSyncException e) =>
                    {
                        SyncError?.Invoke(sender, e);
                    };

                    phoneHelper.PhoneContactsLoaded += (sender, e) => {

                        PhoneContactsLoaded?.Invoke(sender, new EventArgs());
                    };
                }

                return phoneHelper;
            }
        }

		public static async Task LoadPhoneContacts()
        {
            PhoneHelper.LoadPhoneContacts();
        }

        public static List<PhoneContact> GetPhoneContacts()
        {
            return PhoneHelper.GetPhoneContacts();
        }

        public static async Task SyncPhoneContact(BookSyncContact bookSyncContact, CancellationToken token)
        {
            PhoneHelper.SetPhonePicture(bookSyncContact, token);
        }
    }
}
